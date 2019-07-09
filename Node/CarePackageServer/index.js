/**
 * Module dependencies
 */

var express = require('express')
    , http = require('http');
var fs = require('fs');
var mongoose = require('mongoose');
var Schema = mongoose.Schema;


//
//
//

var app = express();
var server = http.createServer(app);
app.use(express.static('public'));

const multer = require('multer');
const upload = multer();

app.get('/view', function (req, res, next) {
    CarePackageImageModel.findById("12345", function (err, doc) {
        if (err) return next(err);

        console.log(doc);

        var data = doc.img.data.toString();
        var base64Data = data.replace(/^data:image\/png;base64,/, '');
        var img = Buffer.from(base64Data, 'base64');

        res.writeHead(200, {
            'Content-Type': 'image/png',
            'Content-Length': img.length
        });
        res.end(img);

    });
});

app.post('/upload', upload.none(), (req, res) => {
    const formData = req.body;
    console.log('form data', formData.file);

    //

    var document = new CarePackageImageModel;
    document._id = "12345";
    document.img.data = formData.file;
    document.img.contentType = 'image/png';

    CarePackageImageModel.findOneAndUpdate(
        { _id: '12345' }, // find a document with that filter
        document, // document to insert when nothing was found
        { upsert: true, new: true, runValidators: true, useFindAndModify: false }, // options
        function (err, doc) { // callback
            if (err) {
                // handle error
            } else {
                // handle document
            }
        }
    );

    //

    res.sendStatus(200);
});

server.on('close', function () {
    console.error('dropping db');
    mongoose.connection.db.dropDatabase(function () {
        console.error('closing db connection');
        mongoose.connection.close();
    });
});

server.listen(3333, function (err) {
    var address = server.address();
    console.error('server listening on http://%s:%d', address.address, address.port);
    console.error('press CTRL+C to exit');
});

//
//
//


// connect to mongo
mongoose.connect('mongodb://localhost:27017/carepackage', { useNewUrlParser: true });

// example schema
var schema = new Schema({
    _id: Number,
    img: { data: Buffer, contentType: String }
});

// our model
var CarePackageImageModel = mongoose.model('A', schema);

mongoose.connection.on('open', function () {
    console.error('mongo is open');

    // empty the collection
    CarePackageImageModel.remove(function (err) {
        if (err) throw err;

        console.error('removed old docs');


    });

});