using UnityEngine;

public class SelectPreset : SimpleBtnAdder
{
    [SerializeField]
    int _index;

    public override void ClickButton()
    {
        var presets = MainManager.Instance.presets;

        if (presets.Contains(_index))
        {
            presets.RemoveAll(o => o == _index);
            EventManager.GetEvent<Color>().Publish(new Color32(150, 150, 150, 255), $"SELECTABLE_{_index}");
        }
        else
        {
            if (_index < 7)
            {
                if (presets.Contains(7))
                {
                    presets.Remove(7);
                    EventManager.GetEvent<Color>().Publish(new Color32(150, 150, 150, 255), "SELECTABLE_7");
                }

                if (presets.Count >= 8)
                {
                    return;
                }

                presets.Add(_index);
                presets.Add(_index);

                EventManager.GetEvent<Color>().Publish(Color.white, $"SELECTABLE_{_index}");
            }
            else if (_index == 7)
            {
                foreach (int preset in presets)
                {
                    EventManager.GetEvent<Color>().Publish(new Color32(150, 150, 150, 255), $"SELECTABLE_{preset}");
                }
                presets.Clear();

                presets.Add(7);
                EventManager.GetEvent<Color>().Publish(Color.white, "SELECTABLE_7");
            }
        }
    }
}
