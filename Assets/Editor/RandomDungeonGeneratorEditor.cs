using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractDungeonGenerator), true)]  // thuộc tính này
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractDungeonGenerator generator;

    private void Awake()
    {
        generator = (AbstractDungeonGenerator)target;   // target là biến có sẵn trong lớp Editor, nó đại diện cho đối tượng đang được chỉnh sửa trong Inspector
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  // vẽ giao diện mặc định của Inspector
        if (GUILayout.Button("Generate Dungeon"))  // tạo một nút trong Inspector
        {
            generator.GenerateDungeon();
        }
    }
}
