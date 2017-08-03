using FIX.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIX.Service
{
    public class DocService : IDocService
    {
        private IUnitOfWork _uow;

        public DocService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public string GetNextDocumentNumber(DBConstant.DBCDocSequence.EDocSequenceId docId)
        {
            var docSeq = _uow.Repository<DocSequence>().GetByKey((int)docId);
            docSeq.Current++;
            _uow.Repository<DocSequence>().Update(docSeq);

            var valWithLeadingZeroes = docSeq.Current.ToString().PadLeft(docSeq.Length, '0');

            var docCode = docSeq.Prefix + valWithLeadingZeroes;

            return docCode;
        }

        public void SaveChange(int userId)
        {
            _uow.Save(userId);
        }


    }
}
