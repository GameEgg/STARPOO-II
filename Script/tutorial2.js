	function update(){

		myShips.forEach(function(myShip,index){

			var targetDegree = index * 360 / myShips.length;
			var targetR = groundRadius * 0.8;

			var targetPoint = cartesian({r:targetR,rot:targetDegree});
			var polarFromShip = polarFrom(myShip,targetPoint);

			myShip.setRotSpeed(polarFromShip.rot/dt);
			myShip.setSpeed(polarFromShip.r / dt);
			
		});
	}