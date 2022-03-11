using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;


public class UIManager : MonoBehaviour
{

    //Ui Elements
    private Slider X_Slider, Y_Slider, Z_Slider;
    private Slider Rot_Slider;
    private Slider Scale_Slider;

    private Button Next_BTN;
    private DropdownField QualityDropdown;
    private DropdownField Varients;

    private Toggle OrbitCamToggle;

    private VisualElement TransformPanel;

    public UIDocument UIDocument;

    // Scene GameObjects
    public GameObject[] SelectedObjectList;
    private int selectedOB;
    public GameObject RootObject;

    private Transform[] InitialObjectTransforms = new Transform[3];
    private Transform InitialRootTransform;

    public ViewerData Data;

    public Cinemachine.CinemachineFreeLook OrbitCam;

    public RenderPipelineAsset[] QualityAssets;

    public GameObject[] PostVolumes;

    private float tempTime;
    
    void Start()
    {
        //Store initial transforms
        for (int i = 0; i < SelectedObjectList.Length; i++)
        {
            InitialObjectTransforms[i] = SelectedObjectList[i].transform;
        }
        InitialRootTransform = RootObject.transform;

        //Initialize UI
        Initialize(UIDocument.rootVisualElement);

        //Misc inits
        tempTime = Time.time;
        QualitySettings.SetQualityLevel(0);
        QualitySettings.renderPipeline = QualityAssets[0];
        PostVolumes[0].SetActive(true);
        PostVolumes[1].SetActive(false);
    }

    public void Initialize(VisualElement root)
    {
        //Init & Event handler of translate X

        X_Slider = root.Q<Slider>("XSlider");

        X_Slider.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            SelectedObjectList[selectedOB].transform.localPosition = new Vector3(evt.newValue, SelectedObjectList[selectedOB].transform.localPosition.y, SelectedObjectList[selectedOB].transform.localPosition.z);
        });

        //Init & Event handler of translate Y

        Y_Slider = root.Q<Slider>("YSlider");

        Y_Slider.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            SelectedObjectList[selectedOB].transform.localPosition = new Vector3(SelectedObjectList[selectedOB].transform.localPosition.x, evt.newValue, SelectedObjectList[selectedOB].transform.localPosition.z);
        });

        //Init & Event handler of translate Z

        Z_Slider = root.Q<Slider>("ZSlider");

        Z_Slider.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            SelectedObjectList[selectedOB].transform.localPosition = new Vector3(SelectedObjectList[selectedOB].transform.localPosition.x, SelectedObjectList[selectedOB].transform.localPosition.y , evt.newValue);
        });

        //Init & Event handler of Rotation

        Rot_Slider = root.Q<Slider>("RotSlider");

        Rot_Slider.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            RootObject.transform.rotation = Quaternion.Euler(RootObject.transform.rotation.eulerAngles.x, evt.newValue, RootObject.transform.rotation.eulerAngles.z);
        });

        //Init & Event handler of Scale

        Scale_Slider = root.Q<Slider>("ScaleSlider");

        Scale_Slider.RegisterCallback<ChangeEvent<float>>((evt) =>
        {
            RootObject.transform.localScale = new Vector3(evt.newValue, evt.newValue, evt.newValue);
        });

        //Init & Event handler of Material Swap

        Varients = root.Q<DropdownField>("DropdownField");

        Varients.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            switch(evt.newValue)
            {
                case "Type 1":
                    if(selectedOB == 0)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB1_Set1;
                    else if (selectedOB == 1)
                    {
                        for (int i = 0; i < SelectedObjectList[selectedOB].transform.childCount; i++)
                        {
                            SelectedObjectList[selectedOB].transform.GetChild(i).GetComponent<MeshRenderer>().materials = Data.OB2_Set1;
                        }
                    }
                    else if (selectedOB == 2)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB3_Set1;
                    break;
                case "Type 2":
                    if (selectedOB == 0)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB1_Set2;

                    else if (selectedOB == 1)
                    {
                        for (int i = 0; i < SelectedObjectList[selectedOB].transform.childCount; i++)
                        {
                            SelectedObjectList[selectedOB].transform.GetChild(i).GetComponent<MeshRenderer>().materials = Data.OB2_Set2;
                        }
                    }
                    else if (selectedOB == 2)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB3_Set2;
                    break;
                case "Type 3":
                    if (selectedOB == 0)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB1_Set3;
                    else if (selectedOB == 1)
                    {
                        for (int i = 0; i < SelectedObjectList[selectedOB].transform.childCount; i++)
                        {
                            SelectedObjectList[selectedOB].transform.GetChild(i).GetComponent<MeshRenderer>().materials = Data.OB2_Set3;
                        }
                    }
                    else if (selectedOB == 2)
                        SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB3_Set3;
                    break;
            }
        });


        //Init & Event handler of switching obects

        Next_BTN = root.Q<Button>("Next_Button");

        Next_BTN.clickable.clicked += () =>
        {
            ResetObject();
            ResetUI();
            SwitchObject();
        };

        TransformPanel = root.Q<VisualElement>("VisualElementTransform");

        //Init & Event handler of Orbit Cam Toggle

        OrbitCamToggle = root.Q<Toggle>("OrbitCamToggle");

        OrbitCamToggle.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            ToggleOrbitCam();
        });

        //Init & Event handler of Quality Settings

        QualityDropdown = root.Q<DropdownField>("Quality");

        QualityDropdown.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            switch (evt.newValue)
            {
                case "High":
                    QualitySettings.SetQualityLevel(0);
                    QualitySettings.renderPipeline = QualityAssets[0];
                    PostVolumes[0].SetActive(true);
                    PostVolumes[1].SetActive(false);
                    break;
                case "Medium":
                    QualitySettings.SetQualityLevel(1);
                    QualitySettings.renderPipeline = QualityAssets[1];
                    PostVolumes[0].SetActive(false);
                    PostVolumes[1].SetActive(true); 
                    break;
                case "Low":
                    QualitySettings.SetQualityLevel(2);
                    QualitySettings.renderPipeline = QualityAssets[2];
                    PostVolumes[0].SetActive(false);
                    PostVolumes[1].SetActive(false);
                    break;
            }
        });
    }

    //Toggles between Orbit and Fixed Cam
    private void ToggleOrbitCam()
    {
        if (OrbitCam.enabled)
        {
            OrbitCam.enabled = false;
            TransformPanel.visible = true;
            
        }
        else
        {
            OrbitCam.enabled = true;
            TransformPanel.visible = false;
            
        }
    }

    private void ResetObject()
    {
        //Reset Transforms
        RootObject.transform.SetPositionAndRotation(InitialRootTransform.position, InitialRootTransform.rotation);
        SelectedObjectList[selectedOB].transform.position = InitialObjectTransforms[selectedOB].position;

        //ResetMaterials
        switch (selectedOB)
        {
            case 0:
                SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB1_Set1;
                break;
            case 1:
                for (int i = 0; i < SelectedObjectList[selectedOB].transform.childCount; i++)
                {
                    SelectedObjectList[selectedOB].transform.GetChild(i).GetComponent<MeshRenderer>().materials = Data.OB2_Set1;
                }
                break;
            case 2:
                SelectedObjectList[selectedOB].GetComponent<MeshRenderer>().materials = Data.OB3_Set1;
                break;
            default:
                break;
        }
    }

    //Resets UI Elements
    private void ResetUI()
    {
        X_Slider.value = 0;
        Y_Slider.value = 0;
        Z_Slider.value = 0;

        Rot_Slider.value = 180;
        Scale_Slider.value = 1;
    }

    //Switch objects in round robin
    private void SwitchObject()
    {
        SelectedObjectList[selectedOB].gameObject.SetActive(false);
        selectedOB++;
        if (selectedOB == 3)
            selectedOB = 0;
        SelectedObjectList[selectedOB].gameObject.SetActive(true);
    }
        
    void Update()
    {
        //Shortkey for toggle between orbit and fixed cam
        if (Input.GetKey(KeyCode.O) && Time.time > tempTime)
        {
            ToggleOrbitCam();
            tempTime = Time.time + 1;
        }
    }
}
