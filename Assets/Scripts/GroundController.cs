using UnityEngine;
using System;
using SocketIO;
using System.Collections;

public class GroundController : MonoBehaviour
{
    private Vector3 euler;

    public float speed;
    public float x = 0;
    public float z = 0;

    public bool androidEnabled = true;
    private Rigidbody rb;
    private bool isOnPlatform = false;

    private SocketIOComponent socket;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        euler = transform.eulerAngles;

        GameObject go = GameObject.Find("SocketIO");
        if (go == null)
            Debug.Log("go is null: " + go);
        else
            Debug.Log("go is not null: " + go);

        socket = go.GetComponent<SocketIOComponent>();

		if (androidEnabled) 
		{
			socket.On("open", TestOpen);
			socket.On("nodeToUnity", TestBoop);
			socket.On("error", TestError);
			socket.On("close", TestClose);

			//StartCoroutine("BeepBoop");
		}


    }

    private IEnumerator BeepBoop()
    {
        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);

        socket.Emit("unityData");
    }



    void FixedUpdate()
    {
        float moveHorizontal;
        float moveVertical;


        if (isOnPlatform && androidEnabled)
        {
			socket.Emit("unityData");
            //transform.Rotate(0, 0.0f, x);

            //Debug.Log("x: " + x);

            euler.x = x;
            euler.z = z;
            transform.eulerAngles = euler;
        }
        else if (isOnPlatform)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");

            // negative sign in front of horizontal only so that way the arrow keys behave intuitively
            transform.Rotate(moveVertical * speed, 0.0f, -moveHorizontal * speed);
    }


    //Vector3 movement = new Vector3(0.0f, moveVertical, 0.0f);
    //rb.AddForce(movement * speed);
}

    public void TestOpen(SocketIOEvent e)
    {
        // e.name has the value of 'open' 
        // e.data is empty
        //Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
    }

    public void TestBoop(SocketIOEvent e)
    {
        // e.data holds the value emitted from the node server
        // e.name holds the header name from the emit 'nodeToUnity' 
        //Debug.Log("received: " + e.name + " " + e.data);

        if (e.data == null)
        {
            Debug.Log("e.data is null");
            return;
        } else
        {
            string data = e.data + "";
            data = data.Substring(1, data.Length - 2);
            char[] delimiter = { ' ' };
            string[] coords = data.Split(delimiter);
            x = Convert.ToSingle(coords[0]);
            z = Convert.ToSingle(coords[1]);
            //Debug.Log("data: " + data);
            //Debug.Log("X value: " + x);
            //Debug.Log("Z value:" + z);
        }

        //Debug.Log(
        //    "#####################################################" +
        //    "THIS: " + e.data.GetField("this").str +
        //    "#####################################################"
        //);
    }

    public void TestError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void TestClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }

    void OnCollisionEnter(Collision collision)
    {
        isOnPlatform = true;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        isOnPlatform = true;

    }

    void OnCollisionExit(Collision collision)
    {
        isOnPlatform = false;
    }

}
