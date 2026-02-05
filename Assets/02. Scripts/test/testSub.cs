public class testSub : abc
{
    void OnEnable()
    {
        testPub.OnPub += EventTrigger;
    }

    void OnDisable()
    {
        testPub.OnPub -= EventTrigger;
    }

    override public void EventTrigger()
    {
        // do something
    }

}
