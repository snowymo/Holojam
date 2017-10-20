var udp = require('dgram');
const holojam = require('holojam-node')(['relay'],'192.168.1.11');
// --------------------creating a udp server --------------------

// creating a udp server
var server = udp.createSocket('udp4');

// emits when any error occurs
server.on('error',function(error){
  console.log('Error: ' + error);
  server.close();
});

valid_trackers = {
	"LHR-1ADE44F5" : "RBTA",
	"LHR-08DE8B86" : "P1RH",
	"LHR-18DD003E" : "TBL",
	"LHR-D5E02F77" : "P1HMD",
	
	"LHR-0DC35B0C" : "RBTB",
	"LHR-08DFFC5D" : "TBL2",
	"LHR-0ADD38C6" : "P2RH",
	"LHR-08C154A6" : "P2HMD"
};

// emits on new datagram msg
server.on('message',function(msg,info){
  //console.log('Data received from client : ' + msg.toString());
  //console.log('Received %d bytes from %s:%d\n',msg.length, info.address, info.port);
  //console.log('Data received from client : ' + msg);
  var jsonObj = JSON.parse(msg.toString());
  var liveObjs = []
  
  //console.log(valid_trackers);
  
  for (var key in jsonObj) {
	if (!jsonObj.hasOwnProperty(key)) {
	  continue;
	}
	if (key == 'time') {
		continue;
	}
	
	//console.log("jsonObj[key].id", jsonObj[key].id);
	var validlabel = valid_trackers[jsonObj[key].id];
	//console.log("validlabel",validlabel);
	if(validlabel == "" || validlabel == undefined)
		continue;
	
	var liveObj = {
                  label: validlabel, 
				  vector3s: [{x: parseFloat(jsonObj[key].x), y: parseFloat(jsonObj[key].y), z: -parseFloat(jsonObj[key].z)}],
				  vector4s: [{x: parseFloat(jsonObj[key].qx), y: parseFloat(jsonObj[key].qy), z: -parseFloat(jsonObj[key].qz), w: -parseFloat(jsonObj[key].qw)}]
               };
    //console.log('live Obj to send' + JSON.stringify(liveObj));
    liveObjs.push(liveObj);
  }
  //console.log(JSON.stringify(liveObjs));
  holojam.Send(holojam.BuildUpdate('Vive',liveObjs));
//sending msg
// server.send(msg,info.port,'localhost',function(error){
//   if(error){
//     client.close();
//   }else{
//     console.log('Data sent !!!');
//   }

// });

});

//emits when socket is ready and listening for datagram msgs
server.on('listening',function(){
  var address = server.address();
  var port = address.port;
  var family = address.family;
  var ipaddr = address.address;
  console.log('Server is listening at port ' + port);
  console.log('Server ip :' + ipaddr);
  console.log('Server is IP4/IP6 : ' + family);
});

//emits after the socket is closed using socket.close();
server.on('close',function(){
  console.log('Socket is closed !');
});

server.bind(10000);

// setTimeout(function(){
// server.close();
// },8000);