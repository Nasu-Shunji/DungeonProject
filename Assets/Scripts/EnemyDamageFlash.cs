using System.Collections;
using UnityEngine;

public class EnemyDamageFlash : MonoBehaviour
{
    [Header("Enemy")]

    //ダメージ通知を受け取るEnemyHealth
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Flash")]

    //ダメージ時に色を変更するRenderer
    [SerializeField] private Renderer[] targetRenderers;

    //ダメージを受けたときの色
    [SerializeField] private Color damageColor = Color.red;

    //赤く表示する時間
    [SerializeField] private float flashDuration = 0.15f;

    //各Rendererの元の色を保存する配列
    private Color[] originalColors;

    //現在実行中の点滅コルーチンを保存
    private Coroutine flashCoroutine;

    private void Awake()
    {
        //RendererがInspectorで設定されていない場合、
        //Enemyとその子オブジェクトから自動取得
        if (targetRenderers == null
            || targetRenderers.Length == 0)
        {
            targetRenderers =
                GetComponentsInChildren<Renderer>();
        }

        //Rendererの数と同じ数だけ、
        //元の色を保存できる配列を作成
        originalColors =
            new Color[targetRenderers.Length];

        //各Rendererのゲーム開始時の色を保存
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalColors[i] =
                targetRenderers[i].material.color;
        }
    }

    private void OnEnable()
    {
        //Enemyがダメージを受けたとき、
        //HandleDamagedを実行するようイベントへ登録
        if (enemyHealth != null)
        {
            enemyHealth.Damaged += HandleDamaged;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            //このスクリプトが無効になったら
            //イベントへの登録を解除
            enemyHealth.Damaged -= HandleDamaged;
        }

        //赤い状態のまま無効にならないよう、
        //元の色へ戻す
        RestoreOriginalColors();
    }

    private void HandleDamaged()
    {
        //前回の点滅がまだ実行中なら停止
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        //新しい点滅処理を開始し、実行中のCoroutineを変数へ保存
        flashCoroutine =
            StartCoroutine(FlashDamageColor());
    }

    private IEnumerator FlashDamageColor()
    {
        //すべてのRendererをダメージ色へ変更
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            targetRenderers[i].material.color =
                damageColor;
        }

        //設定した秒数だけ、このCoroutineの続きを待つ
        yield return new WaitForSeconds(
            flashDuration
        );

        //元の色へ戻す
        RestoreOriginalColors();

        //点滅処理が終了したのでnullへ戻す
        flashCoroutine = null;
    }

    private void RestoreOriginalColors()
    {
        //元の色を保存する配列がまだなければ処理しない
        if (originalColors == null)
        {
            return;
        }

        //各Rendererをゲーム開始時の色へ戻す
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                targetRenderers[i].material.color =
                    originalColors[i];
            }
        }
    }
}