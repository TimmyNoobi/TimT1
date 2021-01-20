//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
// 必要なコンポーネントの列記
	//[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{

		public float animSpeed = 1.5f;				// アニメーション再生速度設定
		public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
		public bool useCurves = true;				// Mecanimでカーブ調整を使うか設定する
		// このスイッチが入っていないとカーブは使われない
		public float useCurvesHeight = 0.5f;		// カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）

		// 以下キャラクターコントローラ用パラメタ
		// 前進速度
		public float forwardSpeed = 7.0f;
		// 後退速度
		public float backwardSpeed = 7.0f;
		// 旋回速度
		public float rotateSpeed = 2.0f;
		// ジャンプ威力
		public float jumpPower = 3.0f; 
		// キャラクターコントローラ（カプセルコライダ）の参照
		private CapsuleCollider col;
		private Rigidbody rb;
		// キャラクターコントローラ（カプセルコライダ）の移動量
		private Vector3 velocity;
		// CapsuleColliderで設定されているコライダのHeiht、Centerの初期値を収める変数
		private float orgColHight;
		private Vector3 orgVectColCenter;
		//private Animator anim;							// キャラにアタッチされるアニメーターへの参照
		//private AnimatorStateInfo currentBaseState;			// base layerで使われる、アニメーターの現在の状態の参照

		private GameObject cameraObject;	// メインカメラへの参照
		
		// アニメーター各ステートへの参照
		//static int idleState = Animator.StringToHash ("Base Layer.Idle");
		//static int locoState = Animator.StringToHash ("Base Layer.Locomotion");
		//static int jumpState = Animator.StringToHash ("Base Layer.Jump");
		//static int restState = Animator.StringToHash ("Base Layer.Rest");

		// 初期化
		void Start ()
		{
			// Animatorコンポーネントを取得する
			//anim = GetComponent<Animator> ();
			// CapsuleColliderコンポーネントを取得する（カプセル型コリジョン）
			col = GetComponent<CapsuleCollider> ();
			rb = GetComponent<Rigidbody> ();
			//メインカメラを取得する
			cameraObject = GameObject.FindWithTag ("MainCamera");
			// CapsuleColliderコンポーネントのHeight、Centerの初期値を保存する
			orgColHight = col.height;
			orgVectColCenter = col.center;




			GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.transform.position = new Vector3(0, 2f, 0);
			cube.transform.Rotate(20,20,20,Space.World);

			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3(0, 3, 0);

			GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			capsule.transform.position = new Vector3(2, 4, 0);

			GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			cylinder.transform.position = new Vector3(-2, 5, 0);
		}
		Transform standardPos;
		public float speed = 3.5f;
		private float X;
		private float Y;
		private void Update()
        {
			if (Input.GetKeyDown(KeyCode.Space))
			{ // スペースキーを入力したら

				rb.AddForce(Vector3.up * (jumpPower), ForceMode.VelocityChange);
				//rb.AddForce(Vector3.up * (jumpPower), ForceMode.Acceleration);
			}
			else if(Input.GetKeyUp(KeyCode.Space))
            {
				rb.AddForce(Vector3.up * (-jumpPower), ForceMode.VelocityChange);
			}


			//standardPos = GameObject.Find("CamPos").transform;


			//カメラをスタートする
			//transform.position = standardPos.position;
			//transform.forward = standardPos.forward;

			if (Input.GetMouseButton(0) || Input.GetButton("Vertical"))
			{
				transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
			}
			else
			{
				transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
			}
		}


        // 以下、メイン処理.リジッドボディと絡めるので、FixedUpdate内で処理を行う.
        void FixedUpdate()
		{
			float h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
			float v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義

			//anim.SetFloat("Speed", v);                          // Animator側で設定している"Speed"パラメタにvを渡す
			//anim.SetFloat("Direction", h);                      // Animator側で設定している"Direction"パラメタにhを渡す
			//anim.speed = animSpeed;                             // Animatorのモーション再生速度に animSpeedを設定する
			//currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // 参照用のステート変数にBase Layer (0)の現在のステートを設定する
																	//rb.useGravity = true;//ジャンプ中に重力を切るので、それ以外は重力の影響を受けるようにする



			// 以下、キャラクターの移動処理
			velocity = new Vector3(0, 0, v);        // 上下のキー入力からZ軸方向の移動量を取得
													// キャラクターのローカル空間での方向に変換
			velocity = transform.TransformDirection(velocity);
			//以下のvの閾値は、Mecanim側のトランジションと一緒に調整する
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

		void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Interaction");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), "Up/Down Arrow : Go Forwald/Go Back");
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), "Left/Right Arrow : Turn Left/Turn Right");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), "Hit Space key while Running : Jump");
			GUI.Label (new Rect (Screen.width - 245, 90, 250, 30), "Hit Spase key while Stopping : Rest");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), "Left Control : Front Camera");
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), "Alt : LookAt Camera");
		}


		// キャラクターのコライダーサイズのリセット関数
		void resetCollider ()
		{
			// コンポーネントのHeight、Centerの初期値を戻す
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}