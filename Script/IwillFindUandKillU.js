var num_detectedNewEnemy = 0;
var num_detectedEnemy = 0;
var arr_current_idle_ships = [];
var arr_busy_ships = [];
var arr_detected_already_enemy_ships = [];
var arr_detected_new_enemy_ships = [];
var b_init = false;
var check = false;
var b_detectednewEnemy = false;
var new_arr_detected_already_enemy_ships = [];

function update() {
    loggable = true;
//    myShips[0].setRotSpeed(360);
    var p = polarFrom(myShips[0],{x:0,y:0});
    log2(p.rot);
    return;
    if (!b_init) {
        init();
    }

    updateDetectedNewEnemyNum();

    // set target for idle ships
    if (b_detectednewEnemy) {
        while (arr_detected_new_enemy_ships.length > 0) {
            arr_busy_ships.push([getBestAgent(), arr_detected_new_enemy_ships.pop()]);
        }
    }

    // move best position for still idle ships
    else {
        if (enemyShips.length == 0) {
            for (var i = 0; i < myShips.length; i++) {
                arroundBorder(myShips[i], i);
            }
        }
    }

    releaseDead ();
    // follow target ship for busy ships
    for (var k = 0; k < arr_busy_ships.length; k++) {
        if (arr_busy_ships[k][0].isCharging) {
            if ((arr_busy_ships[k][0].shootingPower - arr_busy_ships[k][0].chargedPower) < 10 && isTeamKill(arr_busy_ships[k][0])) {
                arr_busy_ships[k][0].setRotSpeed(30);
            }
            else {
                copyMovingofTarget(arr_busy_ships[k][0], arr_busy_ships[k][1]);
            }
        }
        else {
            if (readyToShoot(arr_busy_ships[k][0], arr_busy_ships[k][1])) {
                arr_busy_ships[k][0].shoot(5);
                copyMovingofTarget(arr_busy_ships[k][0], arr_busy_ships[k][1]);
            }

            else {
                focusTarget(arr_busy_ships[k][0], arr_busy_ships[k][1]);
            }
        }
    }
}

function init () {
    for (var i = 0; i < myShips.length; i++) {
        arr_current_idle_ships.push(myShips[i]);
        myShips[i].setSpeed(shipMaxSpeed);
    }

    // for (var i = 0; i < enemyShips.length; i++) {
    //     arr_detected_new_enemy_ships.push(enemyShips[i]);
    // }
    b_init = true;
}

function updateDetectedNewEnemyNum (){
    num_detectedNewEnemy = enemyShips.length - num_detectedEnemy;
    if (num_detectedNewEnemy > 0) {
        b_detectednewEnemy = true;
    }
    num_detectedEnemy = enemyShips.length;

    new_arr_detected_already_enemy_ships = [];

    for (var i = 0; i < enemyShips.length; i++) {
        if (!isExistInArray(arr_detected_already_enemy_ships, enemyShips[i])) {
            if (arr_detected_already_enemy_ships == null) {
                arr_detected_already_enemy_ships = [];
                break;
            }
            arr_detected_new_enemy_ships.push(enemyShips[i]);
        }
        new_arr_detected_already_enemy_ships.push(enemyShips[i])
    }
    arr_detected_already_enemy_ships = [];
    if (new_arr_detected_already_enemy_ships === undefined) {
        log(1);
    }
    if (arr_detected_already_enemy_ships === undefined) {
        log(1);
    }
    arr_detected_already_enemy_ships = new_arr_detected_already_enemy_ships;
}

function getBestAgent () {
    var index_bestAgent;
    var bestAgent;

    for (var i = 0; i < arr_current_idle_ships.length; i++) {
        index_bestAgent = 0;
    }
    bestAgent = arr_current_idle_ships[index_bestAgent];
    arr_current_idle_ships.splice(index_bestAgent, 1);

    return bestAgent;
}

function followTarget (myShip, target) {
    // 90 도 유지
    var p = polarFrom(myShip, target);
    if (-90 == p.rot || p.rot == 90) {
        ;
    }
    else {
        myShip.setRotSpeed(p.rot - 90);
    }
}

function copyMovingofTarget (myShip, target) {
    var p = polarFrom(myShip, target);
    // myShip.setRotSpeed(p.rot);
    myShip.setRotSpeed(Math.min(p.rot * 60, 360));
    if (p.r < 1) {
        myShip.setSpeed(1);
    }
    else {
        myShip.setSpeed(shipMaxSpeed);
    }
}

var loggable = true;
function log2(data){
    if(loggable)
        log(data);
    loggable = false;
}

function focusTarget (myShip, target) {
    var p = polarFrom(myShip, target);
    if(myShip.id % 1000 == 6){
        //log2("p.rot(" + p.rot + ") myShip(" + myShip.x.toFixed(1) + "," + myShip.y.toFixed(1) + ")" + " target(" + target.x.toFixed(1) + "," + target.y.toFixed(1) + ")" );
    }
    myShip.setRotSpeed(Math.min(p.rot * 60, 360));
    myShip.setSpeed((180 - Math.abs(p.rot)) / 180 * shipMaxSpeed);
    if (Math.abs(p.rot) < 5) {
        return true;
    }
    return false;
}

function isExistInArray(array, object) {
    for (var i = 0; i < array.length; i++) {
        if (array[i] == object) {
            array.splice(i, 1);
            return true;
        }
    }
    return false;
}

function translate(ship, dest, speed) {
   var p = polarFrom(ship, dest);
   ship.setAngleSpeed(p.rot);
   ship.setSpeed((180 - Math.abs(p.rot)) / 180 * speed);
}

function readyToShoot(myShip, target) {
    var p = polarFrom(myShip, target);
    if (Math.abs(p.rot) < 5) {
        return true;
    }
    return false;
}

function isTeamKill(myShip){
	for(var i = 0; i < myShips.length; i++){
		if(myShip.id == myShips[i].id)
			continue;
		var res = polarFrom(myShip, myShips[i]);
		
		if(Math.abs(res.angle) < getKillAngleSize(myShip, myShips[i]))
			return true;
	}
	return false;
}

var shipRadius = 1;

function getKillAngleSize( shipFrom, shipTo ){
	var distance = dist(shipFrom, shipTo);
	var rad = Math.asin(shipRadius / distance);
	var deg = r2d(rad);
	return deg;
}

function arroundBorder (myShip, index) {
    // 90 도 유지
    var p = polarFrom(myShip, {x:0, y:0});
    if (-90 == p.rot || p.rot == 90) {
        ;
    }
    else {
        if (myShip.id % 2 == 0){
            myShip.setRotSpeed(p.rot + 80 + (groundRadius - polar(myShip).r) / groundRadius + index);
        }
        else {
            myShip.setRotSpeed(p.rot - 80 - (groundRadius - polar(myShip).r) / groundRadius - index);
        }
    }
}

function releaseDead(){
     for (var k = 0; k < arr_busy_ships.length; k++) {
        if (!isExistInArray(myShips, arr_busy_ships[k][0] || !isExistInArray(enemyShips, arr_busy_ships[k][1]))) {
            arr_busy_ships.splice(k, 1);
        }
     }
}