using UnityEngine;
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

        protected override void LoadComponents()
        {
            base.LoadComponents();

            if (this.saiAuth == null)
                this.saiAuth = this.GetComponentInParent<SaiAuth>();
        }

        protected override void OnBindElements(VisualElement root)
        {
            this.usernameField  = root.Q<TextField>("UsernameField");
            this.passwordField  = root.Q<TextField>("PasswordField");
            this.rememberToggle = root.Q<Toggle>("RememberToggle");
            this.loginButton    = root.Q<Button>("LoginButton");
            this.registerButton = root.Q<Button>("RegisterButton");

            this.loginButton.clicked    += this.OnLoginClicked;
            this.registerButton.clicked += this.OnRegisterClicked;

            this.SubscribeToAuthEvents();
        }

        protected override void OnShow()
        {
            this.LoadCredentialsFromAuth();
            this.HideFeedback();
        }

        // Called by OnShow() and by the Editor inspector button.
        public void LoadCredentialsFromAuth()
        {
            if (this.usernameField == null || this.saiAuth == null) return;

            this.usernameField.SetValueWithoutNotify(this.saiAuth.GetUsername());
            this.passwordField.SetValueWithoutNotify(this.saiAuth.GetPassword());
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
            UIRouter.Instance?.ShowPanel("GamerProgress", addToHistory: false);
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
