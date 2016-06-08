var io = require('socket.io')({
	transports: ['websocket'],
});

io.attach(4567);

console.log('listening**');
var data; 
io.on('connection', function(socket){
	console.log('connected on *:4567');
	
	socket.on('androidData', function(androidData){
		data = androidData; // check if it's faster to stringify the data at this point instead
		console.log('android data received: ' + androidData);
	});
	
	socket.on('unityData', function(){
		socket.emit('nodeToUnity', data + "");
	});
	
	socket.on('disconnect', function(){
    console.log('client disconnected');
  });
})

