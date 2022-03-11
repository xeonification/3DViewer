using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoControls : MonoBehaviour
{
    public Material GizmoX, GizmoY, GizmoZ;
    public GameObject GizmoX_GO, GizmoY_GO, GizmoZ_GO;
    public GameObject Model;
    // Start is called before the first frame update
    void Start()
    {
        GizmoX = GizmoX_GO.GetComponent<MeshRenderer>().material;
        GizmoY = GizmoY_GO.GetComponent<MeshRenderer>().material;
        GizmoZ = GizmoZ_GO.GetComponent<MeshRenderer>().material;
    }

    void MoveModel(int Axis,float val)
    {
        
            switch (Axis)
            {
                case 1:
                    Model.transform.position += new Vector3(val, 0, 0);
                    break;
                case 2:
                    Model.transform.position += new Vector3(0, val, 0);
                    break;
                case 3:
                    Model.transform.position += new Vector3(0, 0, val);
                    break;
                default:
                    break;
            }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetMouseButtonDown(0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if(hit.transform.gameObject.name == "XAxis")
            {
                GizmoX.SetFloat("_Selected", 1);
                if (Input.GetMouseButton(0))
                {
                    MoveModel(1, Input.GetAxis("Mouse X"));
                }
            } else if (hit.transform.gameObject.name == "YAxis")
            {
                GizmoY.SetInt("_Selected", 1);
                if (Input.GetMouseButton(0))
                {
                    MoveModel(2, Input.GetAxis("Mouse Y"));
                }
            }
            else if (hit.transform.gameObject.name == "ZAxis")
            {
                GizmoZ.SetInt("_Selected", 1);
                if (Input.GetMouseButton(0))
                {
                    MoveModel(3, Input.GetAxis("Mouse Y"));
                }
            } 
        }
        else
        {
            GizmoX.SetInt("_Selected", 0);
            GizmoY.SetInt("_Selected", 0);
            GizmoZ.SetInt("_Selected", 0);
        }
    }
}
