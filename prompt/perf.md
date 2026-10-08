## 性能测试与提升

1. 对比所有语言实现性能。js版 https://github.com/sylarlong/iztro 。go py rust 版https://github.com/x-haose/x-iztro。每个语言版本跑10万次bysolar调用，每次通过bysolar得到的盘马上调用10次horoscope，分别测试单线程与多线程并发，统计性能。
2. 分析是否有性能优化空间。
3. 优化后的代码再跑一次测试，形成md文档。