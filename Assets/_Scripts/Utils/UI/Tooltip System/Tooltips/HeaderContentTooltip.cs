using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeaderContentTooltip : Tooltip<HeaderContentTooltipInfo>
{
    public TMP_Text HeaderText;
    public TMP_Text ContentText;

    public override void Show(HeaderContentTooltipInfo info)
    {
        base.Show(info);

        HeaderText.text = info.Header;
        ContentText.text = info.Content;
    }
}

public interface IHeaderContentTooltipInfoProvider : ITooltipInfoProvider<HeaderContentTooltipInfo> { }

public class HeaderContentTooltipInfo : TooltipInfo
{
    public string Header;
    public string Content;

    public HeaderContentTooltipInfo(string header, string content)
    {
        Header = header;
        Content = content;
    }
}
