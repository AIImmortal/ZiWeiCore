## 移植要求

1. 将 https://github.com/sylarlong/iztro 代码移植成c#，农历、干支历计算部分使用nuget tyme4net包 (https://www.nuget.org/packages/tyme4net) 。
2. 只需要移植两个核心函数：
    第一个是astro.bySolar(solarDate,timeIndex,gender)，剩余的参数按照参数默认值对应的流程移植，移植到c#时函数签名是BySolar(DateTime time,int gender)，男性=1，女性=0；
    第二个是horoscope(targetDate,timeIndexOfTarget),移植到c#时函数签名是Horoscope(DateTime time)，只需要移植数据属性的计算，agePalace()、palace()、surroundPalaces()、hasHoroscopeStars()、hasHoroscopeMutagen() 等附加方法不用移植。
3. 移植完成后需要随机找1000种不同日期、性别的数据进行校验，考察iztro结果和移植的c#计算结果是否一样。 