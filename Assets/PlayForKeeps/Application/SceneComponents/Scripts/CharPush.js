#pragma strict


var pushPower = 2.0;

function OnControllerColliderHit (hit : ControllerColliderHit)
{
	var body : Rigidbody = hit.collider.attachedRigidbody;
	
	if (body == null || body.isKinematic)
	return;
	
	if (hit.moveDirection.z < -0.3)
	return;
	
	var pushDir : Vector3 = Vector3 (hit.moveDirection.x, hit.moveDirection.y, 0);
	
	body.velocity = pushDir * pushPower;


}
