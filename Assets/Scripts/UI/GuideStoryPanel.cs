using Gameplay.Guide.Core;
using TMPro;
using UI.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public sealed class GuideStoryPanel : AbstractUIPanel
    {
        [Header("Content")]
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _bodyLabel;
        [SerializeField] private Button _closeButton;

        [InjectOptional] private IUIService _uiService;

        private void OnEnable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            }
        }

        public void SetMetadata(GuideStoryMetadata metadata)
        {
            if (_titleLabel != null)
            {
                _titleLabel.text = metadata.Title;
            }

            if (_bodyLabel != null)
            {
                _bodyLabel.text = metadata.Text;
            }
        }

        private void OnCloseButtonClicked()
        {
            if (_uiService != null)
            {
                _uiService.Close<GuideStoryPanel>();
                return;
            }

            Hide();
        }
    }
}
