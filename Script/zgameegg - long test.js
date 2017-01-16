var normal_offset = 0;
var t = 0;

function normal(ship,ratio){
	var target_pos = {x:0,y:0};
	target_pos.x = (groundRadius-15)*cos(360*ratio);
	target_pos.y = (groundRadius-15)*sin(360*ratio);
	
	if(ship.ammo > 4 && !checkFacingAlly(ship)){
		ship.shoot();
	}

	if(dist(target_pos,ship) > 5){
		lookPos(ship,target_pos);
		ship.setSpeed(5);
		return false;
	}
	else{
		ship.setSpeed(0);
		lookPos(ship,{x:ship.y/groundRadius/4,y:ship.x/groundRadius/4});
		return true;
	}

}

function checkFacingAlly(ship){
	for(var i = 0; i < allyShips.length; ++i){
		var ally = allyShips[i];
		if(ally.x == ship.x && ally.y == ship.y) continue;//log(1111111111111);

		var p = polarFrom(ship,ally);

		if(Math.abs(p.angle) > 80){
			continue;
		}

		if(3 > sin(Math.abs(p.angle))*p.r && p.r < 13){
			// if(ship == myShips[0])
				//log(JSON.stringify(p))
			return p.r;
		}
	}
	return Number.MAX_VALUE;
}

function attackEnemy(ship,index){
	// if(ship == myShips[0])
	// 	log(JSON.stringify(enemyShips));
	// if(ship == myShips[0])
	// 	log(enemyShips.length);
	for(var i = 0; i < enemyShips.length; ++i){
		var enemy = enemyShips[i];
		// if(ship == myShips[0]){
		// 	log(JSON.stringify(enemy));
		// 	log(i);
		// }
		var d = dist(ship,enemy);
		if(d < 3){
			ship.setSpeed(0);
			shootToEnemy(ship,enemy,index);
			return true;
		}
		else if(d < 60){
			ship.setSpeed(5);
			shootToEnemy(ship,enemy,index);
			return true;
		}

	}
	return false;
}

function evade(ship,array){
	var target;
	var nearest = 99999;
	for(var i = 0; i < array.length; ++i){
		var item = array[i];
		var p = polarFrom(item,ship);

		if(Math.abs(p.angle) > 80){
			continue;
		}
		if(1 < sin(Math.abs(p.angle))*p.r || p.r > 4){
			continue;
		}

		if(nearest > p.r){
			nearest = p.r;
			target = item;
		}
	}
	if(target != null){
		var a = polarFrom(ship,target).angle;
		if(a < 0 && a > -90 || a > 90 && a < 180){
			lookPos(ship,target,-90);
		}
		else{
			lookPos(ship,target,90);
		}
		if(Math.abs(a) > 30){
			ship.setSpeed(5);
		}
		else{
			ship.setSpeed(0);
		}
	}
}

function dontSuicide(ship){
	var p = polarFrom(ship,{x:0,y:0});
	if (a.r){

	}
}

function update(){
	//t += dt;
	for (var i = 0; i <myShips.length; ++i) {
		var ship = myShips[i];




		var attacking = attackEnemy(ship,i);
		if(!attacking) 
			normal(ship,i/myShips.length);
		evade(ship,bullets);
		if(ship.ammo == 0){
			evade(ship,allyShips);
			evade(ship,enemyShips);
		}





	}
	// 	// myShips[i].shoot()
	// 	ship = myShips[i]
	// 	ship.shoot()
	// 	ship.setSpeed(5)

	// 	var goodR = 20;
	// 	var goodAngle = 90;

	// 	p = polarFrom(ship,{x:0,y:0});

	// 	if(p.r > goodR){
	// 		goodAngle = 0;
	// 	}
	// 		//log(p.angle)
	// 	ship.setAngleSpeed((p.angle-goodAngle)*50);
	// }
}



function lookAngle(ship,angle){
	angle%=360;
	if(Math.abs(angle-ship.angle) > Math.abs(ship.angle-(angle-360))){
		angle -= 360;
	}
	var v = (angle-ship.angle)/dt;
	ship.setAngleSpeed(v);
	return v < 360;
}

function lookPos(ship,pos,angle){
	if(angle == null){
		angle = 0;
	}
	p = polarFrom(ship,pos);
	var a = p.angle-angle;
	a %= 360;
	if(a > 180){
		a -= 360;
	}
	else if(a < -180){
		a += 360;
	}
	var v = a/dt;
	ship.setAngleSpeed(v);
	return v < 360;
}

function shootToEnemy(ship,enemy,index){
	var pos;
	var distance = dist(ship,enemy);

	if(Math.floor(t/2 + index)%2 === 0 && distance > 2.5) // 특별한 놈들은 어긋나게쏨
		pos = virtualPos(enemy,1.2);
	else // 나머지는 정방향으로 쏨
		pos = enemy;

	var shootable = lookPos(ship,pos);
	if(shootable && distance<5 && distance<checkFacingAlly(ship))
		ship.shoot();
}

function virtualPos(ship,d){
	var v = {
		x:ship.x + cos(ship.angle)*d,
		y:ship.y + sin(ship.angle)*d
	};
	return v;
}



//enemyShips : array<ship>
//allyShips : array<ship>
//myShips : array<ship>
//bullets : array<bulley>

//myship : x,y,angle,speed,angleSpeed,hp,ammo
//ship : x,y,angle,hp
//bullet : x,y,speed,angle

//allyShip : x,y,angle,hp,

//function
//shoot() <- 총알쏨
//setSpeed(number) : 초당 배의 전진속력(0~5)
//setAngleSpeed(number) : 초당 회전속도(-360~360)

//polarFrom(center,target) -> {angle, r}

//dt : delta time
//groundRadius : 경기장 반지름

//cos(n) : degree cos
//sin(n) : degree sin
//d2r(n) : degree to radian
//r2d(n) : radian to degree
//dist(a,b) : distance between a and b
