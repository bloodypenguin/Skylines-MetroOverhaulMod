using UnityEngine;
using System.Collections;

public class SkinTest : MonoBehaviour
{
    public GUISkin thisMetalGUISkin;
    public GUISkin thisOrangeGUISkin;
    public GUISkin thisAmigaGUISkin;
    private Rect rctWindow1;
    private Rect rctWindow2;
    private Rect rctWindow3;
    private Rect rctWindow4;
    private bool blnToggleState = false;
    private float fltSliderValue = 0.5f;
    private float fltScrollerValue = 0.5f;
    private Vector2 scrollPosition = Vector2.zero;

    public struct snNodeArray
    {
        public string itemType, itemName;
        public snNodeArray(string itemType, string itemName)
        {
            this.itemType = itemType;
            this.itemName = itemName;
        }
    }
    private snNodeArray[] testArray = new snNodeArray[20];

    void Awake()
    {
        rctWindow1 = new Rect(20, 20, 320, 400);
        rctWindow2 = new Rect(260, 30, 320, 420);
        rctWindow3 = new Rect(260, 30, 320, 200);
        rctWindow4 = new Rect(360, 20, 320, 400);
        for (int i = 0; i < 19; i++)
        {
            testArray[i].itemType = "node";
            testArray[i].itemName = "Hello" + i;
        }
    }
    void OnGUI()
    {
        GUI.skin = thisOrangeGUISkin;
        rctWindow1 = GUI.Window(0, rctWindow1, DoMyWindow, "Orange Unity", GUI.skin.GetStyle("window"));
        GUI.skin = thisMetalGUISkin;
        rctWindow2 = GUI.Window(1, rctWindow2, DoMyWindow2, "Metal Vista", GUI.skin.GetStyle("window"));
        rctWindow3 = GUI.Window(2, rctWindow3, DoMyWindow4, "Compound Control - Toggle Listbox", GUI.skin.GetStyle("window"));
        GUI.skin = thisAmigaGUISkin;
        rctWindow4 = GUI.Window(3, rctWindow4, DoMyWindow, "Amiga500", GUI.skin.GetStyle("window"));
    }

    void gcListItem(string strItemName)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(strItemName);
        blnToggleState = GUILayout.Toggle(blnToggleState, "");
        GUILayout.EndHorizontal();
    }

    void gcListBox()
    {
        GUILayout.BeginVertical(GUI.skin.GetStyle("box"));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(160), GUILayout.Height(130));
        for (int i = 0; i < 20; i++)
        {
            gcListItem("I'm listItem number " + i);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void DoMyWindow4(int windowID)
    {
        gcListBox();
        GUI.DragWindow();
    }

    void DoMyWindow3(int windowID)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(10, 100, 200, 200), scrollPosition, new Rect(0, 0, 220, 200));
        GUI.Button(new Rect(0, 0, 100, 20), "Top-left");
        GUI.Button(new Rect(120, 0, 100, 20), "Top-right");
        GUI.Button(new Rect(0, 180, 100, 20), "Bottom-left");
        GUI.Button(new Rect(120, 180, 100, 20), "Bottom-right");
        GUI.EndScrollView();
        GUI.DragWindow();
    }

    void DoMyWindow(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Im a Label");
        GUILayout.Space(8);
        GUILayout.Button("Im a Button");
        GUILayout.TextField("Im a textfield");
        GUILayout.TextArea("Im a textfield\nIm the second line\nIm the third line\nIm the fourth line");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle button");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        //Sliders
        GUILayout.BeginHorizontal();
        fltSliderValue = GUILayout.HorizontalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Width(128));
        fltSliderValue = GUILayout.VerticalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Height(50));
        GUILayout.EndHorizontal();
        //Scrollbars
        GUILayout.BeginHorizontal();
        fltScrollerValue = GUILayout.HorizontalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Width(128));
        fltScrollerValue = GUILayout.VerticalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Height(90));
        GUILayout.Box("Im\na\ntest\nBox");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow();
    }

    void DoMyWindow2(int windowID)
    {
        GUILayout.Label("3D Graphics Settings");
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        blnToggleState = GUILayout.Toggle(blnToggleState, "Soft Shadows");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Particle Effects");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        blnToggleState = GUILayout.Toggle(blnToggleState, "Enemy Shadows");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Object Glow");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Button("Im a Button");
        GUILayout.TextField("Im a textfield");
        GUILayout.TextArea("Im a textfield\nIm the second line\nIm the third line\nIm the fourth line");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle button");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        //Sliders
        GUILayout.BeginHorizontal();
        fltSliderValue = GUILayout.HorizontalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Width(128));
        fltSliderValue = GUILayout.VerticalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Height(50));
        GUILayout.EndHorizontal();
        //Scrollbars
        GUILayout.BeginHorizontal();
        fltScrollerValue = GUILayout.HorizontalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Width(128));
        fltScrollerValue = GUILayout.VerticalScrollbar(fltScrollerValue, 0.1f, 0.0f, 1.1f, GUILayout.Height(90));
        GUILayout.Box("Im\na\ntest\nBox");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}
