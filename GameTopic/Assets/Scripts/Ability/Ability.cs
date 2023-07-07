using System;


public class Ability{
    public string name;
    public Action action;
    public Ability(string name, Action action){
        this.name = name;
        this.action = action;
    }
    public void Run(){
        action();
    }
}
