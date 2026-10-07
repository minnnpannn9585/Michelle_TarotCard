using UnityEngine;

public class CharacterDrag : MonoBehaviour
{
    [Header("卡牌引用")]
    public GameObject[] cardObjects;
    
    [Header("视觉效果（可选）")]
    [Range(0.5f, 1f)] public float dragAlpha = 0.7f; // 拖拽时的透明度
    [Tooltip("Shown while free. Leave empty to use the active child sprite.")]
    public GameObject unboundVisual;
    [Tooltip("Shown only when bound to a slot. Leave empty to use the inactive child sprite.")]
    public GameObject boundVisual;

    // 新增：角色的整型ID，用于与 CardSlot.expectedId 比较
    public int characterId = -1;

    private Vector3 originalPos; // 角色初始世界位置（核心：归位用）
    private SpriteRenderer spriteRenderer; // 当前可见的Sprite渲染器
    private Vector3 mouseOffset; // 鼠标与角色的偏移量（避免拖拽时角色瞬移到鼠标位置）
    private bool isDragging = false; // 是否正在拖拽
    // 新增：记录当前角色绑定的卡牌（关键！用于精准重置isFilled）
    private CardSlot currentBoundCardSlot; 

    void Start()
    {
        CacheVisuals();
        originalPos = transform.position;
        currentBoundCardSlot = null;
        ShowUnboundVisual();
    }
    
    void OnMouseDown()
    {
        // 开始拖拽时，先解绑旧卡牌（重置isFilled）
        UnbindCurrentCardSlot();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        mouseOffset = transform.position - mouseWorldPos;
        
        // 标记开始拖拽
        isDragging = true;
        ShowUnboundVisual();
        SetVisibleAlpha(dragAlpha);
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
        SetVisibleAlpha(1f);

        // 标记是否匹配到卡牌
        bool isAttachedToCard = false;

        // 遍历所有卡牌，检测角色是否在卡牌范围内
        foreach (var card in cardObjects)
        {
            // 获取卡牌的碰撞体
            BoxCollider2D cardCollider = card.GetComponent<BoxCollider2D>();
            if (cardCollider == null) continue;

            CardSlot cardSlot = cardCollider.GetComponent<CardSlot>();
            if (cardSlot == null) continue;

            // 核心判断：角色的世界位置是否在卡牌碰撞体内，且卡牌未被占用
            if (cardCollider.OverlapPoint(transform.position) && !cardSlot.isFilled)
            {
                // 角色本体留在卡牌上，方便再次拖走
                transform.position = new Vector3(card.transform.position.x, card.transform.position.y, 0);
                isAttachedToCard = true;
                
                // 绑定新卡牌，并标记为已填充
                currentBoundCardSlot = cardSlot;
                currentBoundCardSlot.isFilled = true;
                // 记录当前卡位的角色引用与ID（用于 int 比较）
                currentBoundCardSlot.currentCharacter = gameObject;
                currentBoundCardSlot.currentCharacterId = characterId;

                ShowBoundVisual();
                AlignBoundVisualToPivot(card.transform);
                CardManager.Instance?.CheckWin();

                break; // 匹配到一个卡牌后停止遍历
            }
        }

        // 若未匹配到任何卡牌，回到初始位置
        if (!isAttachedToCard)
        {
            transform.position = originalPos;
            // 归位时解绑旧卡牌
            UnbindCurrentCardSlot();
        }
    }

    // 可选：重置角色位置（比如重新开始游戏时调用）
    public void ResetCharacterPosition()
    {
        // 重置位置时解绑旧卡牌
        UnbindCurrentCardSlot();
        
        transform.position = originalPos;
        isDragging = false;
        ShowUnboundVisual();
        SetVisibleAlpha(1f);
    }

    // 新增：通用解绑方法（复用逻辑，避免重复代码）
    private void UnbindCurrentCardSlot()
    {
        if (currentBoundCardSlot != null)
        {
            // 将绑定的卡牌重置为未填充，并清除对角色的引用与ID
            currentBoundCardSlot.isFilled = false;
            currentBoundCardSlot.currentCharacter = null;
            currentBoundCardSlot.currentCharacterId = -1;
            // 清空绑定的卡牌
            currentBoundCardSlot = null;
        }

        ShowUnboundVisual();
    }

    void CacheVisuals()
    {
        if (unboundVisual != null && boundVisual != null)
            return;

        foreach (Transform child in transform)
        {
            if (!child.GetComponent<SpriteRenderer>())
                continue;

            if (child.name.IndexOf("OnCard", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (boundVisual == null)
                    boundVisual = child.gameObject;
            }
            else if (unboundVisual == null)
            {
                unboundVisual = child.gameObject;
            }
        }
    }

    void ShowBoundVisual()
    {
        SetVisualVisible(unboundVisual, false);
        SetVisualVisible(boundVisual, true);
        spriteRenderer = GetVisibleRenderer();
    }

    void ShowUnboundVisual()
    {
        SetVisualVisible(boundVisual, false);
        SetVisualVisible(unboundVisual, true);
        spriteRenderer = GetVisibleRenderer();
    }

    static void SetVisualVisible(GameObject visual, bool visible)
    {
        if (visual == null)
            return;

        if (!visual.activeSelf)
            visual.SetActive(true);

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.enabled = visible;
    }

    void AlignBoundVisualToPivot(Transform card)
    {
        if (boundVisual == null || card == null)
            return;

        Transform pivot = FindImagePivot(card);
        if (pivot == null)
            return;

        Vector3 position = pivot.position;
        position.z = boundVisual.transform.position.z;
        boundVisual.transform.position = position;
    }

    static Transform FindImagePivot(Transform card)
    {
        foreach (Transform child in card)
        {
            if (string.Equals(child.name, "ImagePivot", System.StringComparison.OrdinalIgnoreCase))
                return child;
        }

        return null;
    }

    SpriteRenderer GetVisibleRenderer()
    {
        SpriteRenderer boundRenderer = GetEnabledRenderer(boundVisual);
        if (boundRenderer != null)
            return boundRenderer;

        SpriteRenderer unboundRenderer = GetEnabledRenderer(unboundVisual);
        if (unboundRenderer != null)
            return unboundRenderer;

        return GetComponent<SpriteRenderer>();
    }

    static SpriteRenderer GetEnabledRenderer(GameObject visual)
    {
        if (visual == null)
            return null;

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer != null && renderer.enabled)
            return renderer;

        return null;
    }

    void SetVisibleAlpha(float alpha)
    {
        spriteRenderer = GetVisibleRenderer();
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1, 1, 1, alpha);
    }
}