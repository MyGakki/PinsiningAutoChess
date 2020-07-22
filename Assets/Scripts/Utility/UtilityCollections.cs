using System.Collections.Generic;

public class SingletonDic<T, S> where T: IGameBase, new()
    where S: new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                instance.Init();
            }

            return instance;
        }
    }

    protected int count = 0;
    protected Dictionary<int, S> dic = new Dictionary<int, S>();
    
    //创建并增加计数
    protected void Create(out S instance, out int id)
    {
        instance = new S();
        id = count++;
        dic.Add(id, instance);
    }

    public S Get(int id)
    {
        S instance = default(S);
        dic.TryGetValue(id, out instance);
        return instance;
    }
}

//可被计数的对象
public class CountableInstance
{
    public int Id;
}

public class CounterMap<T, S>
    where T : IGameBase, new()
    where S : CountableInstance, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                instance.Init();
            }

            return instance;
        }
    }

    protected int count = 0;
    protected Dictionary<int, S> dic = new Dictionary<int, S>();
    
    //创建并增加计数
    protected S Create()
    {
        S instance = new S();
        instance.Id = ++count;
        dic.Add(instance.Id, instance);

        return instance;
    }
    
    //根据id获取对象
    public S Get(int id)
    {
        S instance = default(S);
        dic.TryGetValue(id, out instance);
        return instance;
    }
}
