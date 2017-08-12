// include plug-ins
var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var del = require('del');
var sass = require('gulp-sass');
var less = require('gulp-less');
var rename = require('gulp-rename');
var minify = require('gulp-clean-css');
var merge = require('merge-stream');
var util = require('gulp-util');
var watch = require('gulp-watch');
var browserify = require('browserify');
var source = require('vinyl-source-stream');
var rev = require('gulp-rev');
var order = require("gulp-order");

var config = {
    //Include all js files but exclude any min.js files
    //prodsrc: ['Scripts/**/*.js', '!Scripts/**/*.min.js'],
    srcJs: {
        'jquery.js': ['Scripts/jquery-*.min.js'],
        'jqueryval.js': ['Scripts/jquery.validate*.min.js'],
        'modernizr.js': ['Scripts/modernizr-*.js'],
        'bootstrap.js': ['Scripts/bootstrap.min.js', 'Scripts/respond.js'],
        'app.js': ['Scripts/app/extension.js', 'Scripts/app/settings.js', 'Scripts/app/*.js']
    },
    
    srcCss: [
        './Content/css/*.css',
        '!./Content/css/loginpage.css',
        '!./Content/css/portal.css'
    ],

    srcsass: [
        './Content/sass/styles.scss',
        './Content/sass/bootstrap.custom.scss'
    ],

    production: !!util.env.production
}


//delete the output file(s)
gulp.task('clean', function (cb) {
    //del is an async function and not a gulp plugin (just standard nodejs)
    //It returns a promise, so make sure you return that from this task function
    //  so gulp knows when the delete is complete
    return del(['./Scripts/script.min.js', './Content/style.css', './Scripts/bundles/*'], cb);
});

gulp.task('style', function () {

    var cssStream = gulp.src(config.srcCss)
        .pipe(concat('style-css.css'))
    ;

    var scssStream = gulp.src(config.srcsass)
        .pipe(sass({ style: 'expanded' }))
        .pipe(concat('style-sass.scss'))
    ;

    var mergedStream = merge(scssStream, cssStream)
        .pipe(order([
            "style-css.css",
            "style-sass.scss"
        ]))
        .pipe(concat('styles.min.css'))
        .pipe(config.production ? minify() : util.noop())
        .pipe(gulp.dest('./Content/'));

    return mergedStream;
});

gulp.task('script', function (done) {

    var buildFolder = './Scripts/bundles/';

    var src = config.srcJs;

    for (var bundle in src) {
        if (src.hasOwnProperty(bundle)) {
            gulp.src(src[bundle])
            .pipe(config.production ? uglify().on('error', function (err) {
                gutil.log(gutil.colors.red('[Error]'), err.toString());
                this.emit('end');
            }) : util.noop())
            .pipe(concat(bundle))
            // revision - implement in future
            //.pipe(rev())
            //.pipe(gulp.dest(buildFolder))
            //.pipe(rev.manifest(gulp.rename(buildFolder + 'rev-manifest.json'), { merge: true, base: buildFolder }))
            .pipe(gulp.dest(buildFolder))
        }
    }
    done();
});

// the task when a file changes
gulp.task('watch', function () {
    watch(['./Content/sass/**/*.scss', './Content/css/**/*.css', './Content/less/**/*.less'], function() {
        gulp.start('style');
    });

    watch(['./Scripts/**/*.js', '!./Scripts/**/*.min.js', '!./Scripts/bundles/*.js'], function () {
        gulp.start('script');
    });
});

gulp.task('browserify', function () {
    return browserify(['./Scripts/app.js']).bundle()
        .pipe(source('app.js'))
        .pipe(gulp.dest('./bundle/'));
});

//Set a default tasks
gulp.task('build', ['clean'], function () {

    if (config.production) console.log('Building production environment...');
    else console.log('Building in debug environment...');
    
    gulp.start('script', 'style', 'watch');
});