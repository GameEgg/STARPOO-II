function getCenter(){
	var x = 0;
	var y = 0;
	for (var i = myShips.length - 1; i >= 0; i--) {
		var ship = myShips[i];
		x += ship.x;
		y += ship.y;
	}
	x /= myShips.length;
	y /= myShips.length;
	return {x:x,y:y};
}

function update(){
	for (var i = myShips.length - 1; i >= 0; i--) {
		// myShips[i].shoot()
		var ship = myShips[i]
		ship.setSpeed(10)

		// var p = polarFrom(ship,getCenter());
		// // var p2 = polarFrom({x:0,y:0},ship);
		// // if(ship.angle != null){
		// // 	log("ship.angle : "+ship.angle)
		// // }
		// // else{
		// // 	log("ship.angle not exist")
		// // }
		// // p = polar(ship)
		// // if(i == 0){
		// // 	log(ship.angle)
		// // 	log(p.angle)
		// // }
		// // ship.setAngleSpeed(360)
		// if(p.angle > 50){
		// 	ship.setAngleSpeed(360);
		// }
		// else if(p.angle < 50){
		// 	ship.setAngleSpeed(-360);
		// }
		// log(JSON.stringify(myShips[i]))
		// myShips[i].name = "a"
	};
}

//aaa