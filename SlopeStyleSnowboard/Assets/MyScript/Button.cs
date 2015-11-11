using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Texture))]
public class Button : MonoBehaviour
{
    class Boundary
    {
        public Vector2 min = Vector2.zero;
        public Vector2 max = Vector2.zero;
    }

    private static Button[] joysticks;
    private static MPJoystick[] mp_joysticks;                  // A static collection of all joysticks
    private static bool enumeratedJoysticks = false;
    private static float tapTimeDelta = 0.3f;				// Time allowed between taps

    public bool touchPad;
    public Vector2 position = Vector2.zero;
    public Rect touchZone;
    public Vector2 deadZone = Vector2.zero;						// Control when position is output
    public bool normalize = false; 							// Normalize output after the dead-zone?
    public int tapCount;

    private int lastFingerId = -1;								// Finger last used for this joystick
    private float tapTimeWindow;							// How much time there is left for a tap to occur
    private Vector2 fingerDownPos;
    //private float fingerDownTime;
    //private float firstDeltaTime = 0.5f;

    private GUITexture gui;
    //public Texture button_not_press;
    //public Texture button_press;
    private Rect defaultRect;								// Default position / extents of the joystick graphic
    private Boundary guiBoundary = new Boundary();			// Boundary for joystick graphic
    private Vector2 guiTouchOffset;						// Offset to apply to touch input
    private Vector2 guiCenter;							// Center of joystick

    public bool button_state_press = false;
    public bool button_single_shot = false;
    private bool can_shoot = false;
    private float time_shoot;


    void Start()
    {
        gui = (GUITexture)GetComponent(typeof(GUITexture));

        //gui.texture = button_not_press;

        defaultRect = gui.pixelInset;
        defaultRect.x += transform.position.x * Screen.width;// + gui.pixelInset.x; // -  Screen.width * 0.5;
        defaultRect.y += transform.position.y * Screen.height;// - Screen.height * 0.5;

        transform.position = Vector3.zero;

        if (touchPad)
        {
            // If a texture has been assigned, then use the rect ferom the gui as our touchZone
            if (gui.texture)
                touchZone = defaultRect;
        }
        else
        {
            guiTouchOffset.x = defaultRect.width * 0.55f;
            guiTouchOffset.y = defaultRect.height * 0.55f;

            // Cache the center of the GUI, since it doesn't change
            guiCenter.x = defaultRect.x + guiTouchOffset.x;
            guiCenter.y = defaultRect.y + guiTouchOffset.y;

            // Let's build the GUI boundary, so we can clamp joystick movement
            guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
            guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
            guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
            guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
        }
    }

    public Vector2 getGUICenter()
    {
        return guiCenter;
    }

    internal void Disable()
    {
        gameObject.active = false;
        //enumeratedJoysticks = false;	
    }

    private void ResetJoystick()
    {
        gui.pixelInset = defaultRect;
        lastFingerId = -1;
        position = Vector2.zero;
        fingerDownPos = Vector2.zero;
    }

    public bool IsFingerDown()
    {
        return (lastFingerId != -1);
    }

    public void LatchedFinger(int fingerId)
    {
        // If another joystick has latched this finger, then we must release it
        if (lastFingerId == fingerId)
            ResetJoystick();
    }
    public bool singleShoot()
    {
        if (can_shoot && IsFingerDown())
        {
            can_shoot = false;
            return true;
        }
        else
            return false;
    }
    public bool longPress()
    {
        return (IsFingerDown());
    }

    private bool checkPosition(Vector3 pos)
    {
        if (touchPad)
        {
            if (touchZone.Contains(pos))
                return true;
        }
        else
        {
            if ((guiBoundary.min.x <= pos.x) && (guiBoundary.max.x >= pos.x))
            {
                if ((guiBoundary.min.y <= pos.x) && (guiBoundary.max.x >= pos.x))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Update()
    {
        if (!enumeratedJoysticks)
        {
            // Collect all joysticks in the game, so we can relay finger latching messages
            joysticks = (Button[])FindObjectsOfType(typeof(Button));
            mp_joysticks = (MPJoystick[])FindObjectsOfType(typeof(MPJoystick));
            enumeratedJoysticks = true;
        }

        int count = Input.touchCount;

        if (tapTimeWindow > 0)
        {
            tapTimeWindow -= Time.deltaTime;
        }
        else
            tapCount = 0;

        if (count == 0)
            ResetJoystick();
        else
        {
            for (int i = 0; i < count; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (lastFingerId != touch.fingerId && checkPosition(touch.position))
                {
                    can_shoot = true;
                }

                bool shouldLatchFinger = false;
                if (touchPad)
                {
                    if (touchZone.Contains(touch.position))
                    {
                        shouldLatchFinger = true;
                    }
                }
                else if (gui.HitTest(touch.position))
                {
                    shouldLatchFinger = true;
                }

                // Latch the finger if this is a new touch
                if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
                {

                    if (count >= 1)
                        button_state_press = true;

                    if (touchPad)
                    {
                        //gui.color.a = 0.15;
                        lastFingerId = touch.fingerId;
                        //fingerDownPos = touch.position;
                        //fingerDownTime = Time.time;
                    }

                    lastFingerId = touch.fingerId;

                    // Accumulate taps if it is within the time window
                    if (tapTimeWindow > 0)
                        tapCount++;
                    else
                    {
                        tapCount = 1;
                        tapTimeWindow = tapTimeDelta;
                    }

                    // Tell other joysticks we've latched this finger
                    //for (  j : Joystick in joysticks )
                    foreach (Button j in joysticks)
                    {
                        if (j != this)
                            j.LatchedFinger(touch.fingerId);
                    }
                    foreach (MPJoystick j in mp_joysticks)
                    {
                        if (j != this)
                            j.LatchedFinger(touch.fingerId);
                    }
                }

                if (lastFingerId == touch.fingerId)
                {
                    // Override the tap count with what the iPhone SDK reports if it is greater
                    // This is a workaround, since the iPhone SDK does not currently track taps
                    // for multiple touches
                    if (touch.tapCount > tapCount)
                        tapCount = touch.tapCount;

                    if (touchPad)
                    {
                        // For a touchpad, let's just set the position directly based on distance from initial touchdown
                        position.x = Mathf.Clamp((touch.position.x - fingerDownPos.x) / (touchZone.width / 2), -1, 1);
                        position.y = Mathf.Clamp((touch.position.y - fingerDownPos.y) / (touchZone.height / 2), -1, 1);
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        ResetJoystick();
                }
            }

            //Put this code in your class if you want manage press button
            //button_state_press change every time you press button
            /*if (button_state_press)
            {
                gui.texture = button_press;
            }
            else
            {
                gui.texture = button_not_press;
            }*/
        }

    }
}