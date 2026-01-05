using UnityEngine;

public class RigidTest : MonoBehaviour
{
    [SerializeField]private Rigidbody2D myRigid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        myRigid.AddForceAtPosition(new Vector2(1, 4), new Vector2(3, 5));//đẩy 1 điểm trên Object (vừa bay vừa xoay)
        myRigid.AddForceY(2, ForceMode2D.Impulse);//Đẩy theo 1 hướng
        //Force Mode
        //Force -> Đẩy từ từ (tính cả khối lượng)
        //impulse -> Đẩy tức thời
        myRigid.AddRelativeForceX(2, ForceMode2D.Impulse); //Thêm lực theo X Cho localSpace 
        myRigid.AddTorque(3); //Momen xoắn / tạo lực quay là xoay
        myRigid.angularDamping = 2; //Lực cản ma sát khi quay
        //angular = quay
        //damping = lực cản ma sát

        myRigid.MovePosition(new Vector2(3, 5)); //Di chuyển Object đến pos 1 cách mượt mà
        myRigid.MoveRotation(90);//xoay đến 1 góc cụ thể
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
