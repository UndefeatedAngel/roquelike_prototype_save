#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BlockoutRoguelike.Adapters;
using BlockoutRoguelike.Core.Person.Controllers.PlayerController;

namespace BlockoutRoguelike.Core.PlayerController
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : Editor
    {
        private bool _showHealthTests = true;
        private bool _showCoinsTests = true;
        private bool _showSaveTests = true;
        
        public override void OnInspectorGUI()
        {
            var playerController = (PlayerController)target;
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject, 
                "_testDamageAmount", "_testHealAmount", "_testCoinAddAmount", "_testCoinSpendAmount");
            
            var showTestsProperty = serializedObject.FindProperty("_showTests");
            if (showTestsProperty != null && showTestsProperty.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Player Tests", EditorStyles.boldLabel);
                
                _showHealthTests = EditorGUILayout.Foldout(_showHealthTests, "Health Tests");
                if (_showHealthTests)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_testDamageAmount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_testHealAmount"));
                    
                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("Test Damage")) playerController.TestTakeDamage();
                    if (GUILayout.Button("Test Heal")) playerController.TestHeal();
                    if (GUILayout.Button("Test Full Recovery")) TestFullRecovery(playerController);
                    if (GUILayout.Button("Test Death Scenario")) TestDeathScenario(playerController);
                }
                
                EditorGUILayout.Space();
                
                _showCoinsTests = EditorGUILayout.Foldout(_showCoinsTests, "Coins Tests");
                if (_showCoinsTests)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_testCoinAddAmount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_testCoinSpendAmount"));
                    
                    EditorGUILayout.Space();
                    
                    if (GUILayout.Button("Test Add Coins")) playerController.TestAddCoins();
                    if (GUILayout.Button("Test Spend Coins")) playerController.TestSpendCoins();
                    if (GUILayout.Button("Test Insufficient Coins")) TestInsufficientCoins(playerController);
                }
                
                EditorGUILayout.Space();
                
                _showSaveTests = EditorGUILayout.Foldout(_showSaveTests, "Save Tests");
                if (_showSaveTests)
                {
                    if (GUILayout.Button("Force Save")) playerController.SaveData();
                    if (GUILayout.Button("Force Load")) ForceLoad(playerController);
                    if (GUILayout.Button("Delete Save Data")) DeleteSaveData();
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void TestFullRecovery(PlayerController playerController)
        {
            if (playerController.PlayerModel != null)
            {
                int healAmount = playerController.PlayerModel.Health.Max - playerController.PlayerModel.Health.Current;
                playerController.Heal(healAmount);
            }
        }
        
        private void TestDeathScenario(PlayerController playerController)
        {
            if (playerController.PlayerModel != null)
            {
                playerController.TakeDamage(playerController.PlayerModel.Health.Current + 10);
            }
        }
        
        private void TestInsufficientCoins(PlayerController playerController)
        {
            if (playerController.PlayerModel != null)
            {
                int currentCoins = playerController.PlayerModel.Coins.Current;
                bool success = playerController.PlayerModel.Coins.TrySpend(currentCoins + 100);
                Debug.Log($"Spend attempt: {success}, Coins remain: {playerController.PlayerModel.Coins.Current}");
                playerController.PlayerModel.Coins.Increase(currentCoins - playerController.PlayerModel.Coins.Current);
            }
        }
        
        private void ForceLoad(PlayerController playerController)
        {
            if (playerController.PlayerModel != null)
            {
                var dataManagerField = typeof(PlayerController).GetField("_dataManager", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var dataManager = dataManagerField?.GetValue(playerController) as PlayerDataManager;
                dataManager?.LoadData();
                Debug.Log("Player data loaded!");
            }
        }
        
        private void DeleteSaveData()
        {
            var dataService = new CoreDataServiceAdapter();
            dataService.DeletePlayerData();
            Debug.Log("Save data deleted!");
        }
    }
}
#endif