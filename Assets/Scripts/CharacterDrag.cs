using UnityEngine;
using TMPro;

public class CharacterDrag : MonoBehaviour
{
    [Header("角色线索配置")]
    public string clueText; // 该角色的线索内容
    public TextMeshProUGUI clueDisplayText;
    
    [Header("卡牌引用")]
    public GameObject[] cardObjects; // 拖入3张卡牌的GameObject
    
    [Header("视觉效果（可选）")]
    [Range(0.5f, 1f)] public float dragAlpha = 0.7f; // 拖拽时的透明度

    private Vector3 originalPos; // 角色初始世界位置（核心：归位用）
    private SpriteRenderer spriteRenderer; // 角色的Sprite渲染器
    private Vector3 mouseOffset; // 鼠标与角色的偏移量（避免拖拽时角色瞬移到鼠标位置）
    private bool isDragging = false; // 是否正在拖拽

    void Start()
    {
        // 初始化组件引用
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 记录角色初始位置（游戏启动时的位置）
        originalPos = transform.position;
        // 初始化透明度
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    // ===== 鼠标交互核心方法（Sprite专属，需Collider2D）=====
    // 1. 鼠标按下（点击角色）：显示线索 + 记录鼠标偏移
    void OnMouseDown()
    {
        // 显示线索
        if (clueDisplayText != null)
        {
            clueDisplayText.text = "hint: " + clueText;
        }
        
        // 计算鼠标与角色的偏移量（关键：拖拽更顺滑）
        // 将屏幕坐标转为世界坐标（Z轴设为0，2D游戏忽略Z轴）
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        mouseOffset = transform.position - mouseWorldPos;
        
        // 标记开始拖拽
        isDragging = true;
        // 拖拽时半透明（提升体验）
        spriteRenderer.color = new Color(1, 1, 1, dragAlpha);
    }

    // 2. 鼠标拖拽中：更新角色位置
    void OnMouseDrag()
    {
        if (!isDragging) return;
        
        // 转换鼠标屏幕坐标到世界坐标（2D游戏Z轴必须设为0）
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        // 更新角色位置（加上偏移量，避免瞬移）
        transform.position = mouseWorldPos + mouseOffset;
    }

    // 3. 鼠标松开：判断是否吸附到卡牌，否则归位
    void OnMouseUp()
    {
        if (!isDragging) return;
        
        // 恢复拖拽状态和透明度
        isDragging = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        // 标记是否匹配到卡牌
        bool isAttachedToCard = false;

        // 遍历所有卡牌，检测角色是否在卡牌范围内
        foreach (var card in cardObjects)
        {
            // 获取卡牌的碰撞体
            BoxCollider2D cardCollider = card.GetComponent<BoxCollider2D>();
            if (cardCollider == null) continue;

            // 核心判断：角色的世界位置是否在卡牌碰撞体内
            if (cardCollider.OverlapPoint(transform.position))
            {
                // 吸附到卡牌中心（Z轴保持0）
                transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 0);
                isAttachedToCard = true;
                break; // 匹配到一个卡牌后停止遍历
            }
        }

        // 若未匹配到任何卡牌，回到初始位置
        if (!isAttachedToCard)
        {
            transform.position = originalPos;
        }
    }

    // 可选：重置角色位置（比如重新开始游戏时调用）
    public void ResetCharacterPosition()
    {
        transform.position = originalPos;
        isDragging = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}