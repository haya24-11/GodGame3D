/// <summary>
/// 隊形の種別。GameManagerのprivate enumからファイル分離し、
/// 将来的に他クラスから参照できるようにする。
/// </summary>
public enum FormationType
{
    None,
    Horizontal,
    Vertical
}
