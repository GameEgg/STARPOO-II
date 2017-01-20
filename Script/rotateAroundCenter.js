

function update(){
	for (var i = myShips.length - 1; i >= 0; i--) {
		var ship = myShips[i]
		ship.setSpeed(shipMaxSpeed)

		var goodR = 20;
		var goodRot = 90;

		var p = polarFrom(ship,{x:0,y:0});

		if(p.r > goodR){
			goodAngle = 0;
		}
		ship.setRotSpeed((p.rot-goodRot)/dt);
	}
}

//enemyShips : array<ship>
//allyShips : array<ship>
//myShips : array<ship>
//bullets : array<bulley>


//ship : x,y,angle,hp
//bullet : x,y,speed,angle

//allyShip : x,y,angle,hp,

//function
//shoot() <- 총알쏨
//setSpeed(number) : 배의 전진속력(0~5)
//setAngleSpeed(number) : 회전속도()

//polarFrom(center,target) -> angle, r