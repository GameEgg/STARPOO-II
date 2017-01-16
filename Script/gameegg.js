var t = 0;
var minusGR = 20;
var radiusIndexAdd = 0;

var forceMoveArray = [];
var forceEvadeArray = [];
var virtualShip = {x:0,y:0,rot:0,id:0};

var centerOfus = {x:0,y:0};
var centerOfenemy = {x:0,y:0,rot:0};
var newMyShips = [];

var inited = false;

var allMia = 0;

function llog(s){
	if(t%10 === 0){
		log(s);
		t++;
	}
}

function init(){
	for (var i = myShips.length - 1; i >= 0; i--) {
		newMyShips[i] = myShips[i];
	}

	updateCenterOfUs();

	newMyShips.sort(function(ship1,ship2){
		var center = {x:0,y:0,rot:polar(centerOfus).rot};
		var p1 = polarFrom(center,ship1);
		var p2 = polarFrom(center,ship2);

		return (p2.rot + 360)%360 - (p1.rot + 360)%360;

	});
}

function update(){
	if(!inited){
		inited = true;
		init();
	}
	updateCenterOfEnemy();
	updateCenterOfUs();

	for (var i = myShips.length - 1; i >= 0; i--) {
		shipUpdate(myShips[i]);
	}
	t++;
	llog(t);
	updateAllMia();
}

function updateAllMia(){
	allMia++;
	if(allMia > 100){
		radiusIndexAdd++;
		allMia = 0;
	}
}


function updateCenterOfEnemy(){
	centerOfenemy.x = 0;
	centerOfenemy.y = 0;
	for (var i = enemyShips.length - 1; i >= 0; i--) {
		var ship = enemyShips[i];
		centerOfenemy.x += ship.x / enemyShips.length;
		centerOfenemy.y += ship.y / enemyShips.length;
	}
}

function updateCenterOfUs(){
	if(centerOfenemy.x == 0 && centerOfenemy.y == 0){
		centerOfus.x = 0;
		centerOfus.y = 0;
		for (var i = myShips.length - 1; i >= 0; i--) {
			var ship = myShips[i];
			centerOfus.x += ship.x / myShips.length;
			centerOfus.y += ship.y / myShips.length;
		}
	}
	else{
		centerOfus.x += (-centerOfenemy.x - centerOfus.x)*dt;
		centerOfus.y += (-centerOfenemy.y - centerOfus.y)*dt;
	}
}

function shipUpdate(ship){
	var p = polarFrom(ship,{x:0,y:0});

	if(forceEvadeArray[ship.id] > 0){
		evade(ship,enemyShips);
		forceEvadeArray[ship.id] -= dt;
		return;
	}

	if(ship.isCharging){
		shipUpdateCharging(ship);
		ship.setSpeed(0);
		ship.setRotSpeed(0);
	}
	else if(ship.delay > 0.05 || forceMoveArray[ship.id] > 0){
		forceMoveArray[ship.id] -= dt;
		shipUpdateDelay(ship);
	}
	else {
		shipUpdateReady(ship);
	}

	if(p.r > groundRadius - 3 && Math.abs(p.rot) > 80){
		ship.setSpeed(0);
	}
}

function shipUpdateCharging(ship){
	shipUpdateDelay(ship);
}

function shipUpdateDelay(ship){
	if(evade(ship,enemyShips)){
		return;
	}
	else {
		basicMove(ship);
	}
}

function shipUpdateReady(ship){
	if(checkTeamBlockEnemy(ship)){

	}
	else{
		shootAuto(ship);
	}
}

function shootAuto(ship){
	if(enemyShips.length === 0){
		if(polar(ship).r > groundRadius*0.35)
			shootTo(ship,{x:0,y:0,hp:5,rot:0},0);
		else
			shootTo(ship,{x:ship.x*2,y:ship.y*2,hp:5,rot:0},0);
	}
	else{
		shootNearestEnemy(ship);
	}
}

function basicMove(ship){


	var i = 0.4 * (newMyShips.indexOf(ship) + radiusIndexAdd - newMyShips.length*0.5) % newMyShips.length;
	var ratio = i / newMyShips.length;
	var centerP = polar(centerOfus);

	var target_pos = {x:0,y:0};
	target_pos.x = (minusGR)*cos(360*ratio + centerP.rot);
	target_pos.y = (minusGR)*sin(360*ratio + centerP.rot);

	if(dist(target_pos,ship) > 3){
		lookPos(ship,target_pos);
		ship.setSpeed(10);
		return true;
	}
	else{
		ship.setSpeed(0);
		lookPos(ship,{x:0,y:0});
		return false;
	}
}

function getNearestBlocker(ship,array){
	var minBlocker;
	var minBlockerDist = 9999;
	for (var i = array.length - 1; i >= 0; i--) {
		var he = array[i];
		if(he.id == ship.id)
			continue;

		var p = polarFrom(ship,he);
		if(isClose(p)){
			if(p.r < minBlockerDist){
				minBlockerDist = p.r;
				minBlocker = he;
			}
		}
	}
	return {blocker:minBlocker, dist:minBlockerDist};
}

function checkTeamBlockMe(ship){
	var mateBlocker = getNearestBlocker(ship,myShips);
	var enemyBlocker = getNearestBlocker(ship,enemyShips);
	return mateBlocker.dist < enemyBlocker.dist;
}
function checkTeamBlockEnemy(ship){
	if(enemyShips.length === 0)
		return false;

	virtualShip.x = ship.x;
	virtualShip.y = ship.y;
	virtualShip.rot = ship.rot;
	virtualShip.id = ship.id;
	for (var i = 0; i < enemyShips.length; i++) {
		var enemy = enemyShips[i];

		var p = polarFrom(ship,enemy);
		virtualShip.rot += p.rot;

		var mateBlocker = getNearestBlocker(virtualShip,myShips);
		if(dist(ship,enemy) > mateBlocker.dist){
		}
		else{
			return false;
		}
	}
	return true;
}

function isClose(p){
	if(Math.abs(p.rot) < 20 && p.r < 3){
		return true;
	}
	var yD = Math.abs(sin(p.rot));
	if(Math.abs(p.rot) < 20 && p.r*yD < 5){
		return true;
	}
	return false;
}

function shootingT(target){
	return {x:target.x,y:target.y,rot:target.shootingRot};
}

function evade(ship,array){
	return false;
	var target;
	for(var i = 0; i < array.length; ++i){
		var item = array[i];
		if(!item.isCharging)
			continue;

		var p = polarFrom(shootingT(item),ship);

		if(Math.abs(p.rot) > 30){
			continue;
		}

		if(isClose(p)){
			target = item;
		}
	}
	if(target != null){
		var a = polarFrom(ship,target).rot;
		if(a < 0 && a > -90 || a > 90 && a < 180){
			lookPos(ship,target,-90);
		}
		else{
			lookPos(ship,target,90);
		}
		if(Math.abs(a) > 80){
			ship.setSpeed(5);
		}
		else{
			ship.setSpeed(0);
		}
		forceEvadeArray[ship.id] = target.shootingPower - target.chargedPower;
		return true;
	}
	return false;
}

function lookPos(ship,pos,rot){
	if(rot == null){
		rot = 0;
	}
	p = polarFrom(ship,pos);
	var a = p.rot-rot;
	a %= 360;
	if(a > 180){
		a -= 360;
	}
	else if(a < -180){
		a += 360;
	}
	var v = a/dt;
	ship.setRotSpeed(v);
	return Math.abs(v) < shipMaxRotSpeed;
}

function shootNearestEnemy(ship){
	var minValue = 36000;
	var minEnemy;
	for(var i = 0; i < enemyShips.length; ++i){
		var p = polarFrom(ship,enemyShips[i]);
		var v = p.rot;
		if(v < minValue){
			minEnemy = enemyShips[i];
		}
	}
	shootTo(ship,minEnemy,0.5);
}

function shootTo(ship,enemy,d){
	ship.setSpeed(0);

	var pos = virtualPos(enemy,d);

	var shootable = lookPos(ship,pos);
	if(shootable){
		if(checkTeamBlockMe(ship)){
			forceMoveArray[ship.id] = 0.5;
			return;
		}
		ship.shoot(Math.min(enemy.hp,shipMaxHp * 0.51));
	}
}

function virtualPos(ship,d){
	var v = {
		x:ship.x + cos(ship.rot)*d,
		y:ship.y + sin(ship.rot)*d
	};
	return v;
}