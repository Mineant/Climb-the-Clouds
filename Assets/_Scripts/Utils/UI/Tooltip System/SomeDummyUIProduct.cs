using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeDummyUIProduct : Product<SomeDummyUIProductArgs>, IHeaderContentTooltipInfoProvider
{
    public HeaderContentTooltipInfo GetTooltipInfo()
    {
        return new("Dummy Header", "Dummy Content blah blah blah");
    }
}

public class SomeDummyUIProductArgs : ProductArgs
{

}
