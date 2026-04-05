using Quest;
using TMPro;
using UnityEngine;

namespace UI
{
    public sealed class VerticalSliceWinScreen : MonoBehaviour
    {
        [SerializeField] private VerticalSliceQuestProgression _questProgression;
        [SerializeField] private GameObject _winPanelRoot;

        private bool _shown;

        private void Awake()
        {
            if (_winPanelRoot != null)
            {
                _winPanelRoot.SetActive(false);
            }

            EnsureMessageLabel();
        }

        private void OnEnable()
        {
            if (_questProgression == null)
            {
                return;
            }

            _questProgression.Completed += OnQuestFinished;
            _questProgression.QuestCompleted += OnQuestFinished;

            if (_questProgression.CurrentStep == VerticalSliceQuestProgression.QuestStep.Completed)
            {
                Show();
            }
        }

        private void OnDisable()
        {
            if (_questProgression == null)
            {
                return;
            }

            _questProgression.Completed -= OnQuestFinished;
            _questProgression.QuestCompleted -= OnQuestFinished;
        }

        private void OnQuestFinished()
        {
            Show();
        }

        private void Show()
        {
            if (_shown)
            {
                return;
            }

            _shown = true;
            if (_winPanelRoot != null)
            {
                _winPanelRoot.SetActive(true);
            }
        }

        private void EnsureMessageLabel()
        {
            if (_winPanelRoot == null)
            {
                return;
            }

            if (_winPanelRoot.GetComponentInChildren<TMP_Text>(true) != null)
            {
                return;
            }

            var go = new GameObject("WinMessage");
            go.transform.SetParent(_winPanelRoot.transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "Trial complete";
            tmp.fontSize = 36;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (TMP_Settings.defaultFontAsset != null)
            {
                tmp.font = TMP_Settings.defaultFontAsset;
            }
        }
    }
}
