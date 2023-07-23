public interface IInfo{

}

public interface IDumpable<T> where T: IInfo{
    public T Dump();
}
public interface ILoadable<T> where T: IInfo{
    public void Load(T info);
}
public interface IStorable: IDumpable<IInfo>, ILoadable<IInfo>{

}

