function update(){
	for(var i = 0; i < myShips.length; ++i){
		var myShip = myShips[i];
		var p = polarFrom(myShip,{x:0,y:0});
		if(p.rot < 0){
			myShip.setRotSpeed(shipMaxRotSpeed);
		}
		else{
			myShip.setRotSpeed(-shipMaxRotSpeed);
		}
		if(Math.abs(p.rot) > 90)
			myShip.setSpeed(shipMaxSpeed);
	}
}