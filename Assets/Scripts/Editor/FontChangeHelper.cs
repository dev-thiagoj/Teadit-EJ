using TMPro;
using UnityEditor;
using UnityEngine;

namespace BMV.Editor
{
    public class FontChangeHelper : EditorWindow
    {
        public TMP_FontAsset novaFonte;

        [MenuItem("Ferramentas/Trocar Todas as Fontes")]
        public static void ShowWindow()
        {
            GetWindow<FontChangeHelper>("Trocar Fontes");
        }

        void OnGUI()
        {
            GUILayout.Label("Selecione a Nova Fonte (TMP)", EditorStyles.boldLabel);

            // Campo para arrastar a fonte
            novaFonte = (TMP_FontAsset)EditorGUILayout.ObjectField("Nova Fonte", novaFonte, typeof(TMP_FontAsset), false);

            if (GUILayout.Button("TROCAR EM TUDO AGORA"))
            {
                if (novaFonte == null)
                {
                    EditorUtility.DisplayDialog("Erro", "Por favor, selecione uma fonte primeiro!", "Ok");
                    return;
                }
                TrocarFontes();
            }
        }

        void TrocarFontes()
        {
            // Encontra todos os objetos de texto na cena (Ativos e Inativos)
            TMP_Text[] todosOsTextos = Resources.FindObjectsOfTypeAll<TMP_Text>();
            int contagem = 0;

            foreach (TMP_Text texto in todosOsTextos)
            {
                // Verifica se o objeto está na cena e não é um asset de projeto
                if (texto.gameObject.scene.rootCount != 0)
                {
                    // Registra a ação para permitir Ctrl+Z (Desfazer)
                    Undo.RecordObject(texto, "Troca de Fonte");

                    texto.font = novaFonte;

                    // Marca o objeto como 'sujo' para garantir que a Unity salve a alteração
                    EditorUtility.SetDirty(texto);
                    contagem++;
                }
            }

            Debug.Log($"Sucesso! Fonte alterada em {contagem} objetos de texto.");
        }
    }
}
