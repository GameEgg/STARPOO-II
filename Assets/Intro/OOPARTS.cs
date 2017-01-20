using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class OOPARTS : MonoBehaviour
{
    public Camera camera;
    public Transform[] objectsParents;
    public AudioClip sfx1;
    public AudioClip sfx2;
    float resetTime = 2;
    float term = 0.1f;
    bool end = false;

    struct Block{
		public GameObject obj;
        public Vector3 pos;
        public Vector3 originScale;
        public Quaternion rotation;
		public float randomRate;
        public float resetTime;
        public float startTime;
        public bool gravity;
        public bool playedAudio;
    }

    Dictionary<GameObject, Block> pieces = new Dictionary<GameObject, Block>();
	List<Block> blocks = new List<Block>();


    float t = 0;
    bool reset = false;

	void Start () {
        foreach (Transform objsPT in objectsParents)
        {
            AddObjects(objsPT);
        }
        var a = gameObject.AddComponent<AudioSource>();
        a.clip = sfx1;
        a.pitch = 1.2f;
        a.Play();
    }

    void AddObjects(Transform objects)
    {
		float t = resetTime;
        foreach (Transform objt in objects)
        {
            var obj = objt.gameObject;
			Block block;
            block.originScale = obj.transform.localScale;
			block.obj = obj;
			block.gravity = false;
            block.pos = obj.transform.position;
            block.rotation = obj.transform.rotation;
			block.randomRate = 1.5f;//GetRandom();
            block.resetTime = resetTime + t;
            block.startTime = t;
            block.playedAudio = false;
            obj.transform.localScale = Vector3.zero;
            t += term;
			blocks.Add(block);



            float rz = Random.value * 1500f;
            float z = -300 + Random.value*2000-1000;
            var forcex = rz * 5;
            var forcey = rz * 3;
            obj.transform.position = obj.transform.position + (new Vector3(GetRandom() * forcex - forcex * 0.75f, GetRandom() * forcey - forcey * 0.75f, z));

            
        }
    }

    float GetRandom()
    {
        return Random.value * 0.5f + 0.5f;
    }
    float GetRandomSpeed()
    {
        var m = (Random.value > 0.5f) ? 1 : -1;
        return m*(Random.value * 0.2f + 0.8f)*36000;
    }

    // Update is called once per frame
    bool gravity = true;
    void OffGravity()
    {
        foreach (GameObject obj in pieces.Keys)
        {
            Block block = pieces[obj];
            obj.GetComponent<Rigidbody>().useGravity = false;
			obj.GetComponent<BoxCollider>().enabled = false;
			obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
			obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            if (obj.transform.childCount == 1)
            {
                obj.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

	void Update () {
        t += Time.deltaTime;
        transform.localRotation = Quaternion.Euler(new Vector3(0, t * 360, 0));
		for (int i = 0; i < blocks.Count; ++i)
        {
			var blk = blocks[i];
			if(t >= blk.resetTime){
                float rate = Mathf.Max(0, Mathf.Min(1, blk.randomRate * (t - blk.resetTime) / 1.2f));
                blk.obj.transform.position = Vector3.Lerp(blk.obj.transform.position, blk.pos, rate);
                blk.obj.transform.rotation = Quaternion.Lerp(blk.obj.transform.rotation, blk.rotation, rate);
                blk.obj.transform.localScale = Vector3.Lerp(blk.obj.transform.localScale, blk.originScale, rate);
            }
			else{
                if( t >= blk.startTime && t < blk.startTime +0.1f)
                {
                    blk.obj.GetComponent<Rigidbody>().angularVelocity = new Vector3(GetRandomSpeed(), GetRandomSpeed(), GetRandomSpeed());
                    
                }
                if (t >= blk.startTime)
                    blk.obj.transform.localScale = 10*blk.originScale * Mathf.Min(1,(2-Mathf.Pow(2, (blk.resetTime-t) / resetTime)));
            }
        }

        if (!end && t > resetTime + term*blocks.Count + 0.5f)
        {
            end = true;
            StartCoroutine(End());
        }
	}
    IEnumerator End()
    {
        var speed = 0f;
        while(camera.transform.position.z < 240)
        {
            speed += 100 * Time.deltaTime;
            var z = camera.transform.position.z + speed * Time.deltaTime;
            var p = camera.transform.position;
            camera.transform.position = new Vector3(p.x, p.y, z);
            yield return null;
        }
        SceneManager.LoadScene("Main");
    }
}
