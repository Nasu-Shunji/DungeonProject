using System.Collections;
using UnityEngine;

public class PlayerDamageFlash : MonoBehaviour
{
    [Header("Player")]

    //ダメージ通知を受け取るPlayerHealth
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Flash")]

    //ダメージ時に色を変更するRenderer、Player本体や正面確認用Cubeなどを登録する
    [SerializeField] private Renderer[] targetRenderers;

    //ダメージを受けた瞬間の色
    [SerializeField] private Color damageColor = Color.red;

    //赤く表示する時間
    [SerializeField] private float flashDuration = 0.15f;

    //各Rendererの元の色を保存する配列
    private Color[] originalColors;

    //現在実行中の点滅処理を保存する
    private Coroutine flashCoroutine;

    private void Awake()
    {
        //RendererがInspectorで設定されていない場合、Playerと子オブジェクトから自動で取得する
        if (targetRenderers == null
            || targetRenderers.Length == 0)
        {
            targetRenderers =
                GetComponentsInChildren<Renderer>();
        }

        //Rendererの数と同じ数だけ、元の色を保存できる配列を作る
        originalColors =
            new Color[targetRenderers.Length];

        //各Rendererが持っているゲーム開始時の色を保存する
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalColors[i] =
                targetRenderers[i].material.color;
        }
    }

    private void OnEnable()
    {
        //Playerがダメージを受けたとき、HandleDamagedを実行するよう登録
        if (playerHealth != null)
        {
            playerHealth.Damaged += HandleDamaged;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            //このスクリプトが無効になったら、ダメージイベントへの登録を解除
            playerHealth.Damaged -= HandleDamaged;
        }

        //スクリプトが無効になったときも元の色へ戻す
        RestoreOriginalColors();
    }

    private void HandleDamaged()
    {
        //前回の点滅処理がまだ実行中なら停止する、
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        //Playerを一時的に赤くする処理を開始、コルーチン：処理を途中で一時停止し、後のフレームで続きを実行できる仕組み
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

        //設定した秒数だけ待つ
        yield return new WaitForSeconds(
            flashDuration
        );

        //すべてのRendererを元の色へ戻す
        RestoreOriginalColors();

        //点滅処理が終了したことを示す
        flashCoroutine = null;
    }

    private void RestoreOriginalColors()
    {
        //配列がまだ作られていなければ処理しない
        if (originalColors == null)
        {
            return;
        }

        //保存しておいた元の色を各Rendererへ戻す
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