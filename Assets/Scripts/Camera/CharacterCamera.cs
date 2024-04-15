using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    [SerializeField]
    GameObject _virtualCameraPrefab;
    CinemachineVirtualCamera _virtualCamera;
    // Start is called before the first frame update

    public void CreateCamera(Transform transform)
    {
        if(_virtualCameraPrefab == null)
        {
            Debug.LogError("Virtual Camera Prefab is not set in CharacterCamera");
            return;
        }
        GameObject cameraObj = Instantiate(_virtualCameraPrefab, transform.position, transform.rotation);
        cameraObj.name = gameObject.name + "Camera";
        _virtualCamera = cameraObj.GetComponentInChildren<CinemachineVirtualCamera>();
        CharacterUnfocus();
        _virtualCamera.Follow = gameObject.transform;
        _virtualCamera.LookAt = gameObject.transform;
        Transform container = GameObject.Find("CharacterCameras").transform;
        cameraObj.transform.SetParent(container);
    }

    public void CreateCamera(GameObject objectToFollow)
    {
        if (_virtualCameraPrefab == null)
        {
            Debug.LogError("Virtual Camera Prefab is not set in CharacterCamera");
            return;
        }
        GameObject cameraObj = Instantiate(_virtualCameraPrefab, objectToFollow.transform.position, objectToFollow.transform.rotation);
        cameraObj.name = objectToFollow.name + "Camera";
        _virtualCamera = cameraObj.GetComponentInChildren<CinemachineVirtualCamera>();
        CharacterUnfocus();
        _virtualCamera.Follow = objectToFollow.transform;
        _virtualCamera.LookAt = objectToFollow.transform;
        Transform container = GameObject.Find("CharacterCameras").transform;
        cameraObj.transform.SetParent(container);
    }

    public void CharacterFocus()
    {
        LayerMask mask = Camera.main.cullingMask;
        mask &= ~(1 << LayerMask.NameToLayer("Blockers"));
        Camera.main.cullingMask = mask;

        if (_virtualCameraPrefab != null)
        {
            _virtualCamera.Priority = 2;
        }
        else
        {
            Debug.LogError("Virtual Camera Prefab is not set in CharacterCamera");
        }
    }

    public void SkillFocus()
    {
        if (_virtualCamera != null)
        {
            _virtualCamera.Priority = 3;
        }
    }

    public void CharacterUnfocus()
    {

        if (_virtualCamera != null)
        {
            _virtualCamera.Priority = 0;
        }
        else
        {
            Debug.LogError("No Virtual Camera found in CharacterCamera");
        }
    }
    public void CameraDestroy()
    {
        if (_virtualCamera != null)
        {
            Destroy(_virtualCamera.gameObject);
        } 
    }

    public void ResetLayerMask()
    {
        LayerMask mask = Camera.main.cullingMask;
        mask |= (1 << LayerMask.NameToLayer("Blockers"));
        Camera.main.cullingMask = mask;
    }
}
