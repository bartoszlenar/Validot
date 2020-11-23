namespace Validot.Tests.Unit.Validation
{
    using System;
    using System.Collections.Generic;

    public class TraversingTestCases
    {
        public class LoopClassA
        {
            private string _id = "A_" + Guid.NewGuid().ToString().Substring(0, 5);

            public LoopClassA A { get; set; }

            public LoopClassB B { get; set; }
        }

        public class LoopClassB
        {
            private string _id = "B_" + Guid.NewGuid().ToString().Substring(0, 5);

            public LoopClassA[] CollectionA { get; set; }

            public LoopClassA A { get; set; }

            public LoopClassB B { get; set; }

            public LoopClassC C { get; set; }

            public LoopClassC FieldC;

            public LoopStructD D;

            public LoopStructD? NullableD;

            public LoopStructD[] CollectionD { get; set; }

            public LoopStructD?[] CollectionNullableD { get; set; }
        }

        public class LoopClassC : LoopClassA
        {
            private string _id = "C_" + Guid.NewGuid().ToString().Substring(0, 5);

            public LoopClassC C { get; set; }
        }

        public struct LoopStructD
        {
            public LoopClassA A { get; set; }

            public LoopStructE E { get; set; }

            public LoopStructE? NullableE { get; set; }
        }

        public struct LoopStructE
        {
            public LoopClassA A { get; set; }
        }

        public static IEnumerable<object[]> Loop_Self()
        {
            Specification<LoopClassA> specification = null;

            specification = c => c.AsModel(specification);

            var model1 = new LoopClassA();
            model1.A = model1;

            yield return new object[] { "self", specification, model1, "", "", typeof(LoopClassA) };
        }

        public static IEnumerable<object[]> Loop_Simple()
        {
            Specification<LoopClassA> specificationA = null;

            Specification<LoopClassB> specificationB = null;

            Specification<LoopClassC> specificationC = null;

            specificationA = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB);

            specificationB = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB)
                .Member(m => m.C, specificationC);

            specificationC = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB)
                .Member(m => m.C, specificationC);

            var model1 = new LoopClassA()
            {
                B = new LoopClassB()
            };

            model1.B.A = model1;

            yield return new object[] { "simple", specificationA, model1, "", "B.A", typeof(LoopClassA) };

            var model2 = new LoopClassA()
            {
                A = new LoopClassA()
                {
                    A = new LoopClassA()
                    {
                        A = new LoopClassA(),
                        B = new LoopClassB()
                    },
                    B = new LoopClassB()
                    {
                        A = new LoopClassA(),
                        B = new LoopClassB()
                    }
                },
                B = new LoopClassB()
            };

            model2.B.A = model2;

            yield return new object[] { "simple_withSides", specificationA, model2, "", "B.A", typeof(LoopClassA) };

            var model3 = new LoopClassA()
            {
                A = new LoopClassA()
                {
                    A = new LoopClassA()
                    {
                        A = new LoopClassA(),
                        B = new LoopClassB()
                    },
                    B = new LoopClassB()
                    {
                        A = new LoopClassA(),
                        B = new LoopClassB()
                    }
                }
            };

            model3.A.B.A.B = model3.A.B;

            yield return new object[] { "simple_nested", specificationA, model3, "A.B", "A.B.A.B", typeof(LoopClassB) };

            var model4 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    C = new LoopClassC()
                    {
                        A = new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                                C = new LoopClassC()
                                {
                                    A = new LoopClassA()
                                    {
                                        B = new LoopClassB()
                                    }
                                }
                            }
                        }
                    }
                }
            };

            model4.B.C.A.B.C.A.B.C = model4.B.C;

            yield return new object[] { "simple_manyClasses", specificationA, model4, "B.C", "B.C.A.B.C.A.B.C", typeof(LoopClassC) };
        }

        public static IEnumerable<object[]> Loop_ThroughMembers()
        {
            Specification<LoopClassA> specificationA = null;

            Specification<LoopClassB> specificationB = null;

            Specification<LoopClassC> specificationC = null;

            Specification<LoopStructD> specificationD = null;

            specificationA = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB);

            specificationB = c => c
                .Optional()
                .Member(m => m.FieldC, specificationC)
                .Member(m => m.D, specificationD)
                .Member(m => m.B, specificationB);

            specificationC = c => c
                .Optional()
                .Member(m => m.B, specificationB);

            specificationD = c => c
                .Member(m => m.A, specificationA);

            var model1 = new LoopClassA();
            model1.A = model1;

            yield return new object[] { "self_member", specificationA, model1, "", "A", typeof(LoopClassA) };

            var model2 = new LoopClassA
            {
                A = new LoopClassA()
                {
                    A = new LoopClassA()
                    {
                        A = new LoopClassA()
                    }
                }
            };

            model2.A.A.A = model2.A;

            yield return new object[] { "self_memberNested", specificationA, model2, "A", "A.A.A", typeof(LoopClassA) };

            var model3 = new LoopClassA
            {
                B = new LoopClassB()
                {
                    D = new LoopStructD()
                }
            };

            model3.B.D.A = model3;

            yield return new object[] { "self_fieldStruct", specificationA, model3, "", "B.D.A", typeof(LoopClassA) };

            var model4 = new LoopClassA
            {
                B = new LoopClassB()
                {
                    FieldC = new LoopClassC()
                }
            };

            model4.B.FieldC.B = model4.B;

            yield return new object[] { "self_fieldClass", specificationA, model4, "B", "B.FieldC.B", typeof(LoopClassB) };
        }

        public static IEnumerable<object[]> Loop_ThroughTypes()
        {
            Specification<LoopClassA> specificationA = null;

            Specification<LoopClassB> specificationB = null;

            Specification<LoopClassC> specificationC = null;

            Specification<LoopStructD> specificationD = null;

            Specification<LoopStructE> specificationE = null;

            specificationA = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB);

            specificationB = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB)
                .Member(m => m.CollectionA, m => m.Optional().AsCollection(specificationA))
                .Member(m => m.C, specificationC)
                .Member(m => m.FieldC, specificationC)
                .Member(m => m.D, specificationD)
                .Member(m => m.CollectionD, m => m.Optional().AsCollection(specificationD))
                .Member(m => m.NullableD, m => m.Optional().AsNullable(specificationD))
                .Member(m => m.CollectionNullableD, m => m.Optional().AsCollection(m1 => m1.Optional().AsNullable(specificationD)));

            specificationC = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB)
                .Member(m => m.C, specificationC);

            specificationD = c => c
                .Member(m => m.A, specificationA)
                .Member(m => m.E, specificationE)
                .Member(m => m.NullableE, m => m.Optional().AsNullable(specificationE));

            specificationE = c => c
                .Member(m => m.A, specificationA);

            var model1 = new LoopClassA()
            {
                B = new LoopClassB()
            };

            model1.B.A = model1;

            yield return new object[] { "types_class", specificationA, model1, "", "B.A", typeof(LoopClassA) };

            var model2 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    D = new LoopStructD()
                }
            };

            model2.B.D.A = model2;

            yield return new object[] { "types_struct", specificationA, model2, "", "B.D.A", typeof(LoopClassA) };

            var model3 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    NullableD = new LoopStructD()
                    {
                        A = new LoopClassA()
                    }
                }
            };

            model3.B.NullableD.Value.A.B = model3.B;

            yield return new object[] { "types_nullable", specificationA, model3, "B", "B.NullableD.A.B", typeof(LoopClassB) };

            var model4 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA(),
                        null,
                        new LoopClassA(),
                    }
                }
            };

            model4.B.CollectionA[2] = model4;

            yield return new object[] { "types_collection", specificationA, model4, "", "B.CollectionA.#2", typeof(LoopClassA) };

            var model5 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionD = new[]
                    {
                        new LoopStructD(),
                        new LoopStructD(),
                        new LoopStructD(),
                        new LoopStructD(),
                        new LoopStructD()
                        {
                            A = new LoopClassA()
                        }
                    },
                }
            };

            model5.B.CollectionD[4].A.B = model5.B;

            yield return new object[] { "types_collection_structs", specificationA, model5, "B", "B.CollectionD.#4.A.B", typeof(LoopClassB) };

            var model6 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionNullableD = new LoopStructD?[]
                    {
                        new LoopStructD()
                        {
                            A = new LoopClassA()
                        }
                    },
                }
            };

            model6.B.CollectionNullableD[0].Value.A.B = model6.B;

            yield return new object[] { "types_collection_nullables", specificationA, model6, "B", "B.CollectionNullableD.#0.A.B", typeof(LoopClassB) };
        }

        public static IEnumerable<object[]> Loop_ThroughIndexes()
        {
            Specification<LoopClassA> specificationA = null;

            Specification<LoopClassB> specificationB = null;

            specificationA = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB);

            specificationB = c => c
                .Optional()
                .Member(m => m.A, specificationA)
                .Member(m => m.B, specificationB)
                .Member(m => m.CollectionA, m => m.AsCollection(specificationA));

            var model1 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                            }
                        }
                    }
                }
            };

            model1.B.CollectionA[1].B.A = model1.B.CollectionA[1];

            yield return new object[] { "indexes_same_amount", specificationA, model1, "B.CollectionA.#1", "B.CollectionA.#1.B.A", typeof(LoopClassA) };

            var model2 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                                CollectionA = new[]
                                {
                                    new LoopClassA()
                                    {
                                        B = new LoopClassB()
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            model2.B.CollectionA[1].B.CollectionA[0].B.A = model2.B.CollectionA[1].B.CollectionA[0];

            yield return new object[] { "indexes_same_amount_nested", specificationA, model2, "B.CollectionA.#1.B.CollectionA.#0", "B.CollectionA.#1.B.CollectionA.#0.B.A", typeof(LoopClassA) };

            var model3 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                                CollectionA = new[]
                                {
                                    new LoopClassA()
                                    {
                                        B = new LoopClassB()
                                        {
                                            A = new LoopClassA()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            model3.B.CollectionA[1].B.CollectionA[0].B.A.B = model3.B.CollectionA[1].B.CollectionA[0].B;

            yield return new object[] { "indexes_same_amount_nested_member", specificationA, model3, "B.CollectionA.#1.B.CollectionA.#0.B", "B.CollectionA.#1.B.CollectionA.#0.B.A.B", typeof(LoopClassB) };

            var model4 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                                CollectionA = new[]
                                {
                                    new LoopClassA()
                                    {
                                        B = new LoopClassB()
                                        {
                                            A = new LoopClassA()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            model4.B.CollectionA[1].B.CollectionA[0].B.A.B = model4.B.CollectionA[1].B;

            yield return new object[] { "indexes_one_between", specificationA, model4, "B.CollectionA.#1.B", "B.CollectionA.#1.B.CollectionA.#0.B.A.B", typeof(LoopClassB) };

            var model5 = new LoopClassA()
            {
                B = new LoopClassB()
                {
                    CollectionA = new[]
                    {
                        new LoopClassA(),
                        new LoopClassA()
                        {
                            B = new LoopClassB()
                            {
                                CollectionA = new[]
                                {
                                    new LoopClassA()
                                    {
                                        B = new LoopClassB()
                                        {
                                            CollectionA = new[]
                                            {
                                                new LoopClassA(),
                                                new LoopClassA(),
                                                new LoopClassA()
                                                {
                                                    B = new LoopClassB()
                                                    {
                                                        A = new LoopClassA()
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            model5.B.CollectionA[1].B.CollectionA[0].B.CollectionA[2].B = model5.B.CollectionA[1].B;

            yield return new object[] { "indexes_two_between", specificationA, model5, "B.CollectionA.#1.B", "B.CollectionA.#1.B.CollectionA.#0.B.CollectionA.#2.B", typeof(LoopClassB) };
        }

        public class TestClassA
        {
            public TestClassA[] CollectionA { get; set; }

            public TestClassB[] CollectionB { get; set; }

            public TestClassA A { get; set; }

            public TestClassB B { get; set; }

            public TestStructC C { get; set; }
        }

        public class TestClassB
        {
            public TestClassA A { get; set; }

            public TestClassB B { get; set; }

            public TestStructC C { get; set; }

            public TestStructC? NullableC { get; set; }
        }

        public struct TestStructC
        {
            public TestClassA A { get; set; }
        }

        public static IEnumerable<object[]> TreesExamples_Common()
        {
            Specification<TestClassB> specificationB = null;

            Specification<TestClassA> specificationA = s => s
                .Member(m => m.A, m => m
                    .AsModel(m1 => m1.Rule(x => false))
                    .Member(m1 => m1.B, m1 => m1.AsModel(m2 => m2.Rule(x => false))))
                .Member(m => m.B, specificationB);

            specificationB = s => s.Member(m => m.A, specificationA);

            var model1 = new TestClassA()
            {
                A = new TestClassA()
                {
                    B = new TestClassB()
                }
            };

            var model2 = new TestClassA()
            {
                A = new TestClassA()
                {
                    B = new TestClassB()
                },
                B = new TestClassB()
                {
                    A = new TestClassA(),
                    B = new TestClassB(),
                    C = new TestStructC(),
                },
                C = new TestStructC(),
            };

            var model3 = new TestClassA()
            {
                A = new TestClassA()
                {
                    B = new TestClassB()
                },
                B = new TestClassB()
                {
                    A = new TestClassA()
                    {
                        A = new TestClassA()
                        {
                            B = new TestClassB()
                        },
                        B = new TestClassB()
                        {
                            A = new TestClassA(),
                            B = new TestClassB(),
                            C = new TestStructC(),
                        },
                        C = new TestStructC()
                        {
                            A = new TestClassA()
                            {
                                A = new TestClassA()
                                {
                                    B = new TestClassB()
                                },
                                B = new TestClassB()
                                {
                                    A = new TestClassA(),
                                    B = new TestClassB(),
                                    C = new TestStructC(),
                                },
                                C = new TestStructC()
                            }
                        }
                    },
                    B = new TestClassB(),
                    C = new TestStructC(),
                },
                C = new TestStructC()
                {
                    A = new TestClassA()
                    {
                        A = new TestClassA()
                        {
                            B = new TestClassB()
                        },
                        B = new TestClassB()
                        {
                            A = new TestClassA(),
                            B = new TestClassB(),
                            C = new TestStructC(),
                        },
                        C = new TestStructC()
                    }
                }
            };

            yield return new object[] { "common1", specificationA, model1 };
            yield return new object[] { "common2", specificationA, model2 };
            yield return new object[] { "common3", specificationA, model3 };
        }

        public static IEnumerable<object[]> TreesExamples_Struct()
        {
            Specification<TestClassA> specificationA = null;
            Specification<TestClassB> specificationB = null;
            Specification<TestStructC> specificationC = null;

            specificationA = s => s.Member(m => m.B, specificationB);
            specificationB = s => s.Member(m => m.C, specificationC);
            specificationC = s => s.Member(m => m.A, specificationA);

            var model1 = new TestClassA()
            {
                A = new TestClassA()
                {
                    B = new TestClassB()
                    {
                        C = new TestStructC()
                        {
                            A = new TestClassA()
                            {
                                B = new TestClassB()
                                {
                                    C = new TestStructC()
                                    {
                                        A = new TestClassA()
                                        {
                                            B = new TestClassB()
                                            {
                                                C = new TestStructC()
                                            }
                                        },
                                    }
                                }
                            },
                        }
                    }
                },
            };

            var model2 = new TestClassA()
            {
                A = new TestClassA()
                {
                    B = new TestClassB()
                    {
                        C = new TestStructC()
                    }
                },
                B = new TestClassB()
                {
                    C = new TestStructC()
                    {
                        A = new TestClassA()
                        {
                            B = new TestClassB()
                            {
                                C = new TestStructC()
                                {
                                    A = new TestClassA()
                                    {
                                        B = new TestClassB()
                                        {
                                            C = new TestStructC()
                                            {
                                                A = new TestClassA()
                                                {
                                                    B = new TestClassB()
                                                    {
                                                        C = new TestStructC()
                                                    }
                                                },
                                            }
                                        }
                                    },
                                }
                            }
                        },
                    }
                },
                C = new TestStructC()
                {
                    A = new TestClassA()
                    {
                        B = new TestClassB()
                    }
                }
            };

            yield return new object[] { "struct1", specificationA, model1 };
            yield return new object[] { "struct2", specificationA, model2 };
        }

        public static IEnumerable<object[]> TreesExamples_Collections()
        {
            Specification<TestClassA> specificationA = null;
            Specification<TestClassB> specificationB = null;

            specificationA = s => s
                .Optional()
                .Member(m => m.CollectionA, m => m.AsCollection(specificationA))
                .Member(m => m.CollectionB, m => m.AsCollection(specificationB));

            specificationB = s => s
                .Optional()
                .Member(m => m.A, specificationA);

            var model31 = new TestClassA()
            {
                CollectionA = new[]
                {
                    new TestClassA(),
                    new TestClassA(),
                    new TestClassA(),
                }
            };

            var model32 = new TestClassA()
            {
                CollectionA = new[]
                {
                    new TestClassA(),
                    new TestClassA(),
                    new TestClassA(),
                },
                CollectionB = new[]
                {
                    new TestClassB(),
                    new TestClassB(),
                    new TestClassB(),
                }
            };

            var model33 = new TestClassA()
            {
                CollectionA = new[]
                {
                    new TestClassA()
                    {
                        CollectionA = new[]
                        {
                            new TestClassA(),
                            new TestClassA(),
                            new TestClassA(),
                        },
                        CollectionB = new[]
                        {
                            new TestClassB(),
                            new TestClassB(),
                            new TestClassB(),
                        }
                    },
                    new TestClassA()
                    {
                        CollectionA = new[]
                        {
                            new TestClassA(),
                            new TestClassA(),
                            new TestClassA(),
                        },
                        CollectionB = new[]
                        {
                            new TestClassB(),
                            new TestClassB(),
                            new TestClassB(),
                        }
                    },
                },
                CollectionB = new[]
                {
                    new TestClassB()
                    {
                        A = new TestClassA()
                        {
                            CollectionA = new[]
                            {
                                new TestClassA(),
                                new TestClassA(),
                                new TestClassA(),
                            },
                            CollectionB = new[]
                            {
                                new TestClassB(),
                                new TestClassB(),
                                new TestClassB(),
                            }
                        },
                    },
                    new TestClassB()
                    {
                        A = new TestClassA()
                        {
                            CollectionA = new[]
                            {
                                new TestClassA(),
                                new TestClassA(),
                                new TestClassA(),
                            },
                            CollectionB = new[]
                            {
                                new TestClassB()
                                {
                                    A = new TestClassA()
                                    {
                                        CollectionA = new[]
                                        {
                                            new TestClassA(),
                                            new TestClassA(),
                                            new TestClassA(),
                                        },
                                        CollectionB = new[]
                                        {
                                            new TestClassB(),
                                            new TestClassB(),
                                            new TestClassB(),
                                        }
                                    },
                                },
                                new TestClassB(),
                                new TestClassB(),
                            }
                        },
                    },
                }
            };

            yield return new object[] { "collection1", specificationA, model31 };
            yield return new object[] { "collection2", specificationA, model32 };
            yield return new object[] { "collection3", specificationA, model33 };
        }

        public static IEnumerable<object[]> TreesExamples_Nullable()
        {
            Specification<TestClassA> specificationA = null;
            Specification<TestClassB> specificationB = null;
            Specification<TestStructC> specificationc = null;

            specificationA = s => s
                .Optional()
                .Member(m => m.B, specificationB);

            specificationB = s => s
                .Optional()
                .Member(m => m.C, specificationc)
                .Member(m => m.NullableC, m => m.AsNullable(specificationc));

            specificationc = s => s
                .Member(m => m.A, specificationA);

            var model1 = new TestClassA()
            {
                B = new TestClassB()
                {
                    NullableC = new TestStructC()
                    {
                        A = new TestClassA()
                    },
                    C = new TestStructC()
                    {
                        A = new TestClassA()
                    }
                }
            };

            var model2 = new TestClassA()
            {
                B = new TestClassB()
                {
                    NullableC = new TestStructC()
                    {
                        A = new TestClassA()
                        {
                            B = new TestClassB()
                            {
                                NullableC = new TestStructC()
                                {
                                    A = new TestClassA()
                                },
                                C = new TestStructC()
                                {
                                    A = new TestClassA()
                                }
                            }
                        },
                    },
                    C = new TestStructC()
                    {
                        A = new TestClassA()
                        {
                            B = new TestClassB()
                            {
                                NullableC = new TestStructC()
                                {
                                    A = new TestClassA()
                                },
                                C = new TestStructC()
                                {
                                    A = new TestClassA()
                                }
                            }
                        }
                    }
                }
            };

            yield return new object[] { "nullable1", specificationA, model1 };
            yield return new object[] { "nullable2", specificationA, model2 };
        }
    }
}
