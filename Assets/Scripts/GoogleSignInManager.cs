// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Firebase;
// using Firebase.Auth;
// using Firebase.Extensions;
// using Google;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class GoogleSignInManager : MonoBehaviour
// {
//     public string GoogleWebApi = "919828006452-nniqb12h192sj1lger8n6lj56gh9h0v0.apps.googleusercontent.com";
//     private GoogleSignInConfiguration configuration;
//
//     private Firebase.DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
//     private Firebase.Auth.FirebaseAuth auth;
//     private Firebase.Auth.FirebaseUser user;
//
//
//     private void Awake()
//     {
//         configuration = new GoogleSignInConfiguration()
//         {
//             WebClientId = GoogleWebApi,
//             RequestIdToken = true
//         };
//     }
//
//     private void Start()
//     {
//         InitFirebase();
//     }
//
//     private void InitFirebase()
//     {
//         auth = FirebaseAuth.DefaultInstance;
//     }
//
//     void GoogleSignInClick()
//     {
//         GoogleSignIn.Configuration = configuration;
//         GoogleSignIn.Configuration.UseGameSignIn = false;
//         GoogleSignIn.Configuration.RequestIdToken = true;
//         GoogleSignIn.Configuration.RequestEmail = true;
//         GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
//
//     }
//
//     private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
//     {
//         if (task.IsFaulted)
//         {
//             Debug.LogError("Fault");
//         } else if (task.IsCanceled)
//         {
//             Debug.LogError("Login Cancel");
//         }
//         else
//         {
//             Firebase.Auth.Credential credential =
//                 Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
//             auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
//             {
//                 if (task.IsFaulted)
//                 {
//                     Debug.LogError("Fault");
//                     return;
//                 }
//
//                 if (task.IsCanceled)
//                 {
//                     Debug.LogError("Login Cancel");
//                     return;
//                 }
//
//                 user = auth.CurrentUser;
//             });
//         }
//     }
// }
