using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NothingAwaits.Cards
{
    public class DragAndReturnSelector : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Settings")] 
        [SerializeField] private Vector2 _offset = new Vector2(0, 50);
        [SerializeField] private float _returnSpeed = 10f;

        private RectTransform _rectTransform;
        private Vector2 _originalAnchoredPosition;
        private bool _isDragging;
        protected IDragAndReturnSelector _selector;
        protected virtual void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (_isDragging && Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                _rectTransform.position = new Vector2(mousePosition.x + _offset.x, mousePosition.y + _offset.y);

                CheckFor3DObjects();
            }

            if (_isDragging && Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartDragging(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            var mousePosition = eventData.position;
            _rectTransform.position = new Vector2(mousePosition.x + _offset.x, mousePosition.y + _offset.y);
            CheckFor3DObjects();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopDragging();
        }

        private void StartDragging(PointerEventData eventData)
        {
            _isDragging = true;

            var mousePosition = eventData.position;
            _rectTransform.position = new Vector2(mousePosition.x + _offset.x, mousePosition.y + _offset.y);
        }

        private void StopDragging()
        {
            _isDragging = false;

            Take();
            _selector?.Clear();
            _selector = null;

            StartCoroutine(ReturnToPosition());
        }

        private IEnumerator ReturnToPosition()
        {
            var elapsedTime = 0f;
            var startPosition = _rectTransform.anchoredPosition;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * _returnSpeed;
                _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, _originalAnchoredPosition, elapsedTime);
                yield return null;
            }

            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }

        private void CheckFor3DObjects()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                var selector = hit.collider.GetComponent<IDragAndReturnSelector>();
                if (selector != null)
                {
                    _selector = selector;
                    _selector.Highlight();
                }
            }
            else
            {
                _selector?.Clear();
            }
        }

        protected virtual void Take()
        {
            Debug.Log("Selector Take");
        }
    }
}