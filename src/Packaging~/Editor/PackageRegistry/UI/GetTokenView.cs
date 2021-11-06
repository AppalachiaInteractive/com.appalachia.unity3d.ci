using Appalachia.CI.Packaging.PackageRegistry.Core;
using Appalachia.CI.Packaging.PackageRegistry.NPM;
using UnityEditor;
using UnityEngine;

namespace Appalachia.CI.Packaging.PackageRegistry.UI
{
    internal class GetTokenView : EditorWindow
    {
        private static readonly TokenMethod[] methods =
        {
            new("npm login", "Registry username", "Registry password", GetNPMLoginToken),
            new("bintray", "Bintray username", "Bintray API key", GetBintrayToken)

            // TODO adjust TokenMethod to allow for opening GitHub token URL: https://github.com/settings/tokens/new
        };

        private static string error;

        private bool initialized;

        private ScopedRegistry registry;
        private string password;

        private string username;

        private TokenMethod tokenMethod;

        private void CloseWindow()
        {
            error = null;
            foreach (var view in Resources.FindObjectsOfTypeAll<CredentialEditorView>())
            {
                view.Repaint();
            }

            Close();
            GUIUtility.ExitGUI();
        }

        private void OnDisable()
        {
            initialized = false;
        }

        private void OnEnable()
        {
            error = null;
        }

        private void OnGUI()
        {
            if (initialized)
            {
                EditorGUILayout.LabelField(tokenMethod, EditorStyles.whiteLargeLabel);
                username = EditorGUILayout.TextField(tokenMethod.usernameName, username);
                password = EditorGUILayout.PasswordField(tokenMethod.passwordName, password);

                if (GUILayout.Button("Login"))
                {
                    if (tokenMethod.action(registry, username, password))
                    {
                        CloseWindow();
                    }
                }

                if (GUILayout.Button("Close"))
                {
                    CloseWindow();
                }

                if (!string.IsNullOrEmpty(error))
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }
        }

        private void SetRegistry(TokenMethod tokenMethod, ScopedRegistry registry)
        {
            this.tokenMethod = tokenMethod;
            this.registry = registry;
            initialized = true;
        }

        internal static int CreateGUI(int selectedIndex, ScopedRegistry registry)
        {
            EditorGUILayout.LabelField("Generate token", EditorStyles.whiteLargeLabel);
            EditorGUILayout.BeginHorizontal();

            // ReSharper disable once CoVariantArrayConversion
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Method"), selectedIndex, methods);

            if (GUILayout.Button("Login & get auth token"))
            {
                CreateWindow(methods[selectedIndex], registry);
            }

            EditorGUILayout.EndHorizontal();

            return selectedIndex;
        }

        private static void CreateWindow(TokenMethod method, ScopedRegistry registry)
        {
            var getTokenView = GetWindow<GetTokenView>(true, "Get token", true);
            getTokenView.SetRegistry(method, registry);
        }

        private static bool GetBintrayToken(ScopedRegistry registry, string username, string password)
        {
            registry.token = NPMLogin.GetBintrayToken(username, password);
            return !string.IsNullOrEmpty(registry.token);
        }

        private static bool GetNPMLoginToken(ScopedRegistry registry, string username, string password)
        {
            var response = NPMLogin.GetLoginToken(registry.url, username, password);

            if (string.IsNullOrEmpty(response.ok))
            {
                // EditorUtility.DisplayDialog("Cannot get token", response.error, "Ok");
                error = "Cannot get token: " + response.error;
                return false;
            }

            registry.token = response.token;
            return true;
        }
    }
}