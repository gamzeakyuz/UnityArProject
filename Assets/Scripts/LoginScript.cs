using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Firebase.Extensions;

public class LoginScript : MonoBehaviour
{
    public GameObject GirisPanel, KaydolPanel, ProfilePanel, SifreUnutmaPanel, UyariMesajPaneli;

    public InputField forgetPassEmail, loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName;

    public Text uyariTitle, uyariMesaj, profileUserName, profileEmail;

    public Toggle RememberMe;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;


    bool isSingIn = false;


    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void OpenLoginPanel()
    {
        GirisPanel.SetActive(true);
        KaydolPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        SifreUnutmaPanel.SetActive(false);
    }
    public void OpenSignUpPanel()
    {
        GirisPanel.SetActive(false);
        KaydolPanel.SetActive(true);
        ProfilePanel.SetActive(false);
        SifreUnutmaPanel.SetActive(false);
    }
    public void OpenProfilePanel()
    {
        GirisPanel.SetActive(false);
        KaydolPanel.SetActive(false);
        ProfilePanel.SetActive(true);
        SifreUnutmaPanel.SetActive(false);
    }
    public void OpenForgetPassword()
    {
        GirisPanel.SetActive(false);
        KaydolPanel.SetActive(false);
        ProfilePanel.SetActive(false);
        SifreUnutmaPanel.SetActive(true);
    }
    //
    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text) && string.IsNullOrEmpty(loginPassword.text))
        {
            showNotification("Uyarı", "E-posta ve Şifre alanı boş bırakılamaz!");
            return;
        }
        //Do Login
        SignInUser(loginEmail.text, loginPassword.text);

    }
    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) && string.IsNullOrEmpty(signupPassword.text) && string.IsNullOrEmpty(signupCPassword.text) && string.IsNullOrEmpty(signupUserName.text))
        {
            showNotification("Uyarı", "Boş alan bırakılamaz!");
            return;
        }

        //Do signUp

        createUser(signupEmail.text, signupPassword.text, signupUserName.text);

    }

    public void forgetPass()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            showNotification("Uyarı", "E-posta alanı boş bırakılmaz!");
            return;
        }

        forgetPasswordSubmit(forgetPassEmail.text);

    }

    private void showNotification(string title, string message)
    {

        uyariTitle.text = "" + title;
        uyariMesaj.text = "" + message;

        UyariMesajPaneli.SetActive(true);
    }
    public void closeNotificationPanel()
    {

        uyariTitle.text = "";
        uyariMesaj.text = "";

        UyariMesajPaneli.SetActive(false);
    }

    public void LogOut()
    {
        auth.SignOut();
        profileUserName.text = "";
        profileEmail.text = "";
        OpenLoginPanel();
    }

    void createUser(string email, string password, string Username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {

                Exception exception = task.Exception;

                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                //foreach(Exception exception in task.Exception.Flatten().InnerException){
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    showNotification("Uyarı", HataMesajlari(errorCode));

                }
                //}

                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            UpdateUserProfile(Username);

        });
    }

    private void showNotification(string v, IEnumerator enumerator)
    {
        throw new NotImplementedException();
    }

    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Exception exception = task.Exception;
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                //foreach(Exception exception in task.Exception.Flatten().InnerException){
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    showNotification("Uyarı", HataMesajlari(errorCode));
                }
                //}

                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);

            profileUserName.text = "" + newUser.DisplayName;
            profileEmail.text = "" + newUser.Email;

            OpenProfilePanel();

        });
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSingIn = true;
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void UpdateUserProfile(string UserName)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = UserName,
                PhotoUrl = new System.Uri("https://via.placeholder.com/150C/0%20httos://placeholder.com/"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("User profile updated successfully.");

                showNotification("Uyarı", "Hesap Başarıyla Oluşturuldu");

            });
        }

    }

    bool isSigned = false;

    void Update()
    {
        if (isSingIn)
        {
            if (!isSigned)
            {
                isSigned = true;
                profileUserName.text = "" + user.DisplayName;
                profileEmail.text = "" + user.Email;
                OpenProfilePanel();
            }
        }
    }



    private string HataMesajlari(AuthError errorCode)
    {

        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Hesap Mevcut Değil!";
                break;
            case AuthError.MissingPassword:
                message = "Eksik Şifre!";
                break;
            case AuthError.WeakPassword:
                message = "Şifre çok kısa!";
                break;
            case AuthError.WrongPassword:
                message = "Geçersiz Şifre!";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Bu e-posta zaten kullanımda!";
                break;
            case AuthError.InvalidEmail:
                message = "Geçersiz E-posta Adresi!";
                break;
            case AuthError.MissingEmail:
                message = "Eksik E-posta Adresi!";
                break;
            default:
                message = "Geçersiz Hata!";
                break;
        }
        return message;
    }


    void forgetPasswordSubmit(string forgetPasswordEmail)
    {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task => {

            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled");
            }
            if (task.IsFaulted)
            {
                Exception exception = task.Exception;
                //foreach(Exception exception in task.Exception.Flatten().InnerException){
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    var errorCode = (AuthError)firebaseEx.ErrorCode;
                    showNotification("Uyarı", HataMesajlari(errorCode));
                }
                //}

            }

            showNotification("Bilgilendirme", "Yeni şifre için mail adresinize bağlantı başarıyla gönderildi.");

        }
        );
    }
}
