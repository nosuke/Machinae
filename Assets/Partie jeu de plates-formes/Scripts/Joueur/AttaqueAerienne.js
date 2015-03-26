#pragma strict

public var DashDist : float = 0.8f;
var _canDash : boolean = true;

function Update() {
	if (Input.GetKeyDown(KeyCode.X) && _canDash) {
		_canDash = false;
		Dash();
	}
}

function Dash() {
	var DashPos : Vector2;
    var t : float = 0f;
    
    DashPos.x = transform.position.x + DashDist;
    while (t<1) {
	    t += Time.deltaTime*2;
	    transform.position = Vector2.Lerp(transform.position,DashPos,t);
	    yield;
    }
	_canDash = true;
}