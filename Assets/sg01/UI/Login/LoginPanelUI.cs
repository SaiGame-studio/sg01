using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SaiGame.Services;
using SaiGame.UI;

namespace Sg01.UI
{
    // Login panel for sg01 — binds UI Toolkit elements to SaiAuth.
    public class LoginPanelUI : UIPanelBase
    {
        public override string PanelId => "Login";

        [Header("References")]
        [SerializeField] private SaiAuth saiAuth;

        private TextField usernameField;
        private TextField passwordField;
        private Toggle    rememberToggle;
        private Button    loginButton;
        private Button    registerButton;

        // Fallback document when Panel Asset is not LoginPanel.uxml.
        private UIDocument uiDocument;

        protected override void LoadComponents()
        {
            base.LoadComponents();

            if (this.saiAuth == null)
                this.saiAuth = this.GetComponentInParent<SaiAuth>();

            if (this.uiDocument == null)
                this.uiDocument = this.GetComponent<UIDocument>();
        }

        protected override void OnBindElements(VisualElement root)
        {
            this.usernameField  = root.Q<TextField>("UsernameField");
            this.passwordField  = root.Q<TextField>("PasswordField");
            this.rememberToggle = root.Q<Toggle>("RememberToggle");
            this.loginButton    = root.Q<Button>("LoginButton");
            this.registerButton = root.Q<Button>("RegisterButton");

            if (this.loginButton != null)
                this.loginButton.clicked += this.OnLoginClicked;

            if (this.registerButton != null)
                this.registerButton.clicked += this.OnRegisterClicked;

            this.SubscribeToAuthEvents();
        }

        // Handles standalone UIDocument usage (no UIRouter in scene).
        protected override void Start()
        {
            base.Start();

            // If UIRouter never called Initialize(), bind directly from UIDocument.
            if (this.Root == null)
            {
                this.BindFromDocument();
                this.SubscribeToAuthEvents();
            }
        }

        protected override void OnShow()
        {
            this.LoadCredentialsFromAuth();
            this.HideFeedback();
        }

        // Called by OnShow() and by the Editor inspector button.
        public void LoadCredentialsFromAuth()
        {
            // If fields were not bound via UIPanelBase (e.g. Panel Asset is wrong),
            // fall back to the UIDocument on this GameObject.
            if (this.usernameField == null)
                this.BindFromDocument();

            if (this.usernameField == null || this.saiAuth == null) return;

            this.usernameField.SetValueWithoutNotify(this.saiAuth.GetUsername());
            this.passwordField.SetValueWithoutNotify(this.saiAuth.GetPassword());
        }

        // Binds fields directly from the UIDocument's visual tree.
        private void BindFromDocument()
        {
            if (this.uiDocument == null) return;

            VisualElement docRoot = this.uiDocument.rootVisualElement;
            if (docRoot == null) return;

            this.usernameField  = docRoot.Q<TextField>("UsernameField");
            this.passwordField  = docRoot.Q<TextField>("PasswordField");
            this.rememberToggle = docRoot.Q<Toggle>("RememberToggle");

            if (this.loginButton == null)
            {
                this.loginButton    = docRoot.Q<Button>("LoginButton");
                this.registerButton = docRoot.Q<Button>("RegisterButton");

                if (this.loginButton != null)
                    this.loginButton.clicked += this.OnLoginClicked;

                if (this.registerButton != null)
                    this.registerButton.clicked += this.OnRegisterClicked;
            }
        }

        // ------------------------------------------------------------------
        //  Event subscriptions
        // ------------------------------------------------------------------

        private void SubscribeToAuthEvents()
        {
            if (this.saiAuth == null) return;
            this.saiAuth.OnLoginSuccess += this.HandleLoginSuccess;
            this.saiAuth.OnLoginFailure += this.HandleLoginFailure;
        }

        private void UnsubscribeFromAuthEvents()
        {
            if (this.saiAuth == null) return;
            this.saiAuth.OnLoginSuccess -= this.HandleLoginSuccess;
            this.saiAuth.OnLoginFailure -= this.HandleLoginFailure;
        }

        // ------------------------------------------------------------------
        //  Button handlers
        // ------------------------------------------------------------------

        private void OnLoginClicked()
        {
            this.HideFeedback();
            this.loginButton.SetEnabled(false);

            this.saiAuth?.Login(
                this.usernameField.value,
                this.passwordField.value,
                onSuccess: _ => this.loginButton.SetEnabled(true),
                onError:   _ => this.loginButton.SetEnabled(true));
        }

        private void OnRegisterClicked()
        {
            UIRouter.Instance?.ShowPanel("Register");
        }

        // ------------------------------------------------------------------
        //  Auth event handlers
        // ------------------------------------------------------------------

        private void HandleLoginSuccess(LoginResponse response)
        {
            this.ShowFeedback($"Welcome, {response.user?.username}!", isError: false);
            SceneManager.LoadScene("2-game-menu", LoadSceneMode.Single);
        }

        private void HandleLoginFailure(string error)
        {
            this.ShowFeedback(error, isError: true);
        }

        // ------------------------------------------------------------------
        //  Cleanup
        // ------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            this.UnsubscribeFromAuthEvents();

            if (this.loginButton != null)
                this.loginButton.clicked -= this.OnLoginClicked;

            if (this.registerButton != null)
                this.registerButton.clicked -= this.OnRegisterClicked;
        }
    }
}
