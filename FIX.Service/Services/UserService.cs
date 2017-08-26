using System.Linq;
using System.Data.Entity;
using System;
using FIX.Service.Entities;
using System.Threading.Tasks;
using static FIX.Service.DBConstant;
using FIX.Service.Utils;
using System.Configuration;

namespace FIX.Service
{
    public class UserService : IUserService
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IUnitOfWork _uow;

        public const string CIPHER_KEYPHRASE = "k@ikNighT";
        public const int TOKEN_RAND_LENGTH = 20;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IQueryable<Role> GetAllRoles()
        {
            return _uow.Repository<Role>().GetAsQueryable();
        }

        public Role GetUserRoleBy(UserProfile userProfile)
        {
            return _uow.Repository<Role>().GetByKey(userProfile.RoleId);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _uow.Repository<User>().GetAsQueryable();
        }

        public IQueryable<User> GetAllUsersWithoutAdmin()
        {
            return _uow.Repository<User>().GetAsQueryable(filter: x=> x.UserProfile.Role.Description != DBCRole.Admin);
        }

        public IQueryable<UserBankAccount> GetAllUserBankAccount(int? userId)
        {
            return _uow.Repository<UserBankAccount>().GetAsQueryable().Where(x => x.UserId == userId);
        }

        public User GetUserBy(string username)
        {
           return _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.Email == email).FirstOrDefault();
        }

        public User GetReferralBy(int? id)
        {
            var referralId = _uow.Repository<UserProfile>().GetByKey(id).ReferralId;
            return GetUserBy(referralId);
        }

        public User GetUserBy(int? id)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.UserId == id).FirstOrDefault();
        }

        public async Task<bool> IsValid(string username, string password, string keyPhrase)
        {
            var task = Task.Run(() =>
            {
                var user = _uow.Repository<User>().GetAsQueryable().Where(x => x.Username == username && x.StatusId != (int)EStatus.Deactivated).FirstOrDefault();
                if (user != null)
                {
                    try
                    {
                        return SecurePasswordHasher.Verify(password, user.Password);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                        return false;
                    }
                }
                return false;
            });
            return await task;
        }

        public bool IsValidEmailAddress(string email)
        {
            return _uow.Repository<User>().GetAsQueryable().Where(x => x.Email == email).FirstOrDefault() == null;
        }

        public string CreateNewToken(User user, EAccessTokenPurpose purpose)
        {
            if (user != null)
            {
                try
                {
                    var token = Randomizor.GenerateRandomAlphanumeric(TOKEN_RAND_LENGTH);

                    string tokenString = StringCipher.Encrypt(token, CIPHER_KEYPHRASE);

                    var encodedToken = System.Text.Encoding.Unicode.GetBytes(tokenString);

                    var tokenUrlEncoded = System.Web.HttpServerUtility.UrlTokenEncode(encodedToken);

                    var purposeName = Enum.GetName(typeof(EAccessTokenPurpose), purpose);
                    //delete all previously request of the same purpose
                    var tokens = _uow.Repository<AccessToken>().GetAsQueryable(x => x.UserId == user.UserId && x.Purpose == purposeName);
                    _uow.Repository<AccessToken>().DeleteAll(tokens);

                    var newToken = new AccessToken
                    {
                        TokenId = Guid.NewGuid(),
                        UserId = user.UserId,
                        TokenKey = token,
                        CreatedTimestamp = DateTime.UtcNow,
                        Purpose = purposeName,
                        StatusId = (int)EStatus.Active,
                        ExpiredTimestamp = DateTime.UtcNow.AddDays(1)
                    };
                    _uow.Repository<AccessToken>().Insert(newToken);
                    SaveChanges();
                    return tokenUrlEncoded;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
            }

            throw new NullReferenceException("User cannot be null on creating a new token.");
        }

        //token string passed in has userid combined, please seperate and validate token only.
        public AccessToken IsValidToken(string tokenString, EAccessTokenPurpose purpose)
        {
            try
            {
                var tokenPurpose = Enum.GetName(typeof(EAccessTokenPurpose), purpose);

                var tokenUrlDecoded = System.Web.HttpServerUtility.UrlTokenDecode(tokenString);
                var decodedToken = System.Text.Encoding.Unicode.GetString(tokenUrlDecoded);
                var decryptedTokenString = StringCipher.Decrypt(decodedToken, CIPHER_KEYPHRASE);

                var tokenRecord = _uow.Repository<AccessToken>().GetAsQueryable(x => x.TokenKey == decryptedTokenString).FirstOrDefault();

                if (tokenRecord != null)
                {
                    return tokenRecord;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public User ActivateUserAccount(string tokenString)
        {
            var token = IsValidToken(tokenString, EAccessTokenPurpose.VerifyEmail);

            if (token != null)
            {
                var user = token.User;

                //find all request for verifyemail and delete it.
                var similarTokens = _uow.Repository<AccessToken>().GetAsQueryable().Where(x => x.UserId == user.UserId && x.Purpose == token.Purpose);
                _uow.Repository<AccessToken>().DeleteAll(similarTokens);

                if (user != null)
                {
                    user.HasEmailVerified = true;
                    UpdateUser(user);
                    SaveChanges(user.UserId);
                    return user;
                }
                
            }

            return null;
        }

        public IQueryable<User> GetReferralChildren(int? id)
        {
            return GetAllUsers().Where(x => x.UserProfile.ReferralId == id);
        }

        public IQueryable<User> GetUsersWithoutAdmin()
        {
            return GetAllUsers().Where(x => x.UserProfile.RoleId != (int)DBCRole.Id.Admin);
        }

        public void InsertUser(User user)
        {
            user.CreatedTimestamp = DateTime.UtcNow;
            user.Password = SecurePasswordHasher.Hash(user.Password);
            _uow.Repository<User>().Insert(user);
        }

        public void UpdateUser(User user)
        {
            user.ModifiedTimestamp = DateTime.UtcNow;
            _uow.Repository<User>().Update(user);
        }

        public bool ResetPassword(string tokenString, string newPassword)
        {
            var token = IsValidToken(tokenString, EAccessTokenPurpose.ResetPassword);

            if (token != null)
            {
                var user = token.User;

                //find all request for verifyemail and delete it.
                var similarTokens = _uow.Repository<AccessToken>().GetAsQueryable().Where(x => x.UserId == user.UserId && x.Purpose == token.Purpose);
                _uow.Repository<AccessToken>().DeleteAll(similarTokens);

                if (user != null)
                {
                    user.Password = SecurePasswordHasher.Hash(newPassword);
                    _uow.Repository<User>().Update(user);
                    return SaveChanges(user.UserId);
                }

            }

            return false;
        }

        public bool SaveChanges(int userId)
        {
            return _uow.Save(userId);
        }

        public void SaveChanges()
        {
            _uow.Save();
        }

    }
}