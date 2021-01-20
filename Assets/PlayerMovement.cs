using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{


	public float lookSmoother = 3.0f;           // a smoothing setting for camera motion
	public bool useCurves = true;               // Mecanimでカーブ調整を使うか設定する
												// このスイッチが入っていないとカーブは使われない
	public float useCurvesHeight = 0.5f;        // カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）


	public float forwardSpeed = 7.0f;

	public float backwardSpeed = 7.0f;

	public float rotateSpeed = 2.0f;

	public float jumpPower = 3.0f;

	private CapsuleCollider col;
	private Rigidbody rb;

	private Vector3 velocity;
	private Vector3 velocityup;


	private GameObject cameraObject;



	void Start()
    {

		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();

		cameraObject = GameObject.FindWithTag("MainCamera");






		GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0, 2f, 0);
		cube.transform.Rotate(20, 20, 20, Space.World);

		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere.transform.position = new Vector3(0, 3, 0);

		GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		capsule.transform.position = new Vector3(2, 4, 0);

		GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cylinder.transform.position = new Vector3(-2, 5, 0);


	}
	public float speed = 3.5f;
	public Camera thisc;
	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{ // スペースキーを入力したら
			//transform.localPosition += Vector3.up * (jumpPower);
			rb.AddForce(Vector3.up * (jumpPower), ForceMode.VelocityChange);
			//rb.AddForce(Vector3.up * (jumpPower), ForceMode.Acceleration);
		}
		else if (Input.GetKeyUp(KeyCode.Space))
		{
			rb.AddForce(Vector3.up * (-jumpPower), ForceMode.VelocityChange);
		}



		if (Input.GetMouseButton(0) || Input.GetButton("Vertical"))
		{
			transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
		}
		else
		{
			transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
		}

	}

	void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
		float v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義



		velocity = new Vector3(0, 0, v);        // 上下のキー入力からZ軸方向の移動量を取得
												// キャラクターのローカル空間での方向に変換
		velocity = transform.TransformDirection(velocity);

		if (v > 0.1)
		{
			velocity *= forwardSpeed;       // 移動速度を掛ける
		}
		else if (v < -0.1)
		{
			velocity *= backwardSpeed;  // 移動速度を掛ける
		}




		// 上下のキー入力でキャラクターを移動させる
		transform.localPosition += velocity * Time.fixedDeltaTime;

		// 左右のキー入力でキャラクタをY軸で旋回させる
		transform.Rotate(0, h * rotateSpeed, 0);

	}
}
