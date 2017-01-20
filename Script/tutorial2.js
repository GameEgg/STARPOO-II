function update(){

	myShips.forEach(function(myShip,index){
		if(enemyShips.length == 0)
			searchEnemy(myShip,index);
		else
			attackEnemy(myShip);
	});
}

function searchEnemy(myShip,index){
	var targetDegree = index * 360 / myShips.length;
	var targetR = groundRadius * 0.8;

	var targetPoint = cartesian({r:targetR,rot:targetDegree});
	var polarFromShip = polarFrom(myShip,targetPoint);

	myShip.setRotSpeed(polarFromShip.rot/dt);
	myShip.setSpeed(polarFromShip.r / dt);
}

function attackEnemy(myShip){
	myShip.setSpeed(0);

	var targetPoint = enemyShips[0];
	var polarFromShip = polarFrom(myShip,targetPoint);
	myShip.setRotSpeed(polarFromShip.rot/dt);

	if(Math.abs(polarFromShip.rot) < 0.01){
		myShip.shoot(shipMaxHp/2);
	}
}

