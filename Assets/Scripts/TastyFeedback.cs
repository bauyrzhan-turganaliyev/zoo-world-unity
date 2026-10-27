
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animal))]
public sealed class TastyFeedback : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private Camera arenaCamera;

    [SerializeField, Min(0.1f)]
    private float duration = 0.9f;

    [SerializeField, Min(0f)]
    private float downScreenOffset = 0.9f;

    [SerializeField, Min(0f)]
    private float heightOffset = 0.8f;

    [SerializeField, Min(0.05f)]
    private float textHeight = 0.65f;

    private Animal _animal;
    private float _hideAt;
    private bool _visible;
    private bool _sizeInitialized;

    private void Awake()
    {
        _animal = GetComponent<Animal>();

        if (arenaCamera == null)
            arenaCamera = Camera.main;

        if (label == null)
            label = GetComponentInChildren<TextMeshPro>(true);

        if (label == null)
        {
            Debug.LogError(
                "TastyFeedback: TMP label is missing", this);
            return;
        }
        
        label.transform.SetParent(null, false);
        label.transform.localScale = Vector3.one;

        label.text = "Tasty!";
        label.fontStyle = FontStyles.Normal;
        label.alignment = TextAlignmentOptions.Center;
        label.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _animal.Ate += Show;
    }

    private void OnDisable()
    {
        _animal.Ate -= Show;
        Hide();
    }

    private void Show()
    {
        if (label == null)
            return;

        _hideAt = Time.time + duration;
        _visible = true;

        label.gameObject.SetActive(true);
        UpdatePosition();

        if (!_sizeInitialized)
        {
            label.transform.localScale = Vector3.one;
            label.ForceMeshUpdate();

            float glyphHeight = label.textBounds.size.y;
            float parentScale = Mathf.Max(
                Mathf.Abs(label.transform.lossyScale.y),
                0.0001f);

            if (glyphHeight > 0.0001f)
            {
                label.transform.localScale =
                    Vector3.one *
                    (textHeight / (glyphHeight * parentScale));
            }

            _sizeInitialized = true;
        }
    }

    private void LateUpdate()
    {
        if (!_visible)
            return;

        if (Time.time >= _hideAt)
        {
            Hide();
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (arenaCamera == null || label == null)
            return;

        Vector3 screenUp = Vector3.ProjectOnPlane(
            arenaCamera.transform.up,
            Vector3.up);

        Vector3 screenDown = screenUp.sqrMagnitude > 0.0001f
            ? -screenUp.normalized
            : Vector3.back;

        label.transform.position =
            transform.position
            + Vector3.up * heightOffset
            + screenDown * downScreenOffset;

        label.transform.rotation =
            arenaCamera.transform.rotation;
    }

    private void Hide()
    {
        _visible = false;

        if (label != null)
            label.gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (label != null)
            Destroy(label.gameObject);
    }
}
