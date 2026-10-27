using TMPro;
using UnityEngine;

public sealed class TastyFeedback : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField, Min(0.1f)] private float duration = 0.9f;
    [SerializeField, Min(0f)] private float downScreenOffset = 0.9f;
    [SerializeField, Min(0f)] private float heightOffset = 0.8f;
    [SerializeField, Min(0.05f)] private float textHeight = 0.45f;

    private Camera _arenaCamera;
    private float _hideAt;

    private void Awake()
    {
        if (_arenaCamera == null)
            _arenaCamera = Camera.main;

        if (label == null)
            return;

        label.transform.SetParent(null, true);
        label.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (label == null)
            return;

        label.text = "Tasty!";
        label.gameObject.SetActive(true);
        label.ForceMeshUpdate();

        float glyphHeight = label.textBounds.size.y;
        if (glyphHeight > 0.0001f)
            label.transform.localScale = Vector3.one * (textHeight / glyphHeight);

        _hideAt = Time.time + duration;
        UpdateLabelTransform();
    }

    private void LateUpdate()
    {
        if (label == null || !label.gameObject.activeSelf)
            return;

        if (Time.time >= _hideAt)
        {
            label.gameObject.SetActive(false);
            return;
        }

        UpdateLabelTransform();
    }

    private void UpdateLabelTransform()
    {
        if (_arenaCamera == null)
            return;

        Vector3 screenUp = Vector3.ProjectOnPlane(_arenaCamera.transform.up, Vector3.up);
        Vector3 screenDown = screenUp.sqrMagnitude > 0.0001f
            ? -screenUp.normalized
            : Vector3.back;

        label.transform.position = transform.position
                                   + Vector3.up * heightOffset
                                   + screenDown * downScreenOffset;

        label.transform.rotation = _arenaCamera.transform.rotation;
    }

    private void OnDestroy()
    {
        if (label != null)
            Destroy(label.gameObject);
    }
}