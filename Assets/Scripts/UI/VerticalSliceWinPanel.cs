using TMPro;
using UI.Runtime;
using UnityEngine;

namespace UI
{
    public sealed class VerticalSliceWinPanel : AbstractUIPanel
    {
        protected override void Awake()
        {
            base.Awake();
            EnsureMessageLabel();
        }

        private void EnsureMessageLabel()
        {
            var panelRoot = GetPanelRoot();
            if (panelRoot == null)
            {
                return;
            }

            if (panelRoot.GetComponentInChildren<TMP_Text>(true) != null)
            {
                return;
            }

            var go = new GameObject("WinMessage");
            go.transform.SetParent(panelRoot.transform, false);
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
