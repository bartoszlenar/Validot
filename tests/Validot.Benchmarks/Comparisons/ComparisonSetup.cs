namespace Validot.Benchmarks.Comparisons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bogus;
    using Bogus.Extensions;

    using FluentValidation;

    public static class ComparisonSetup
    {

        static ComparisonSetup()
        {
            void SetupValidotSpecifications()
            {
                 NestedModelSpecification = _ => _
                    .Member(m => m.Number1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N1"))
                    .Member(m => m.Number2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message N2"))
                    .Member(m => m.SuperNumber1, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S1"))
                    .Member(m => m.SuperNumber2, m2 => m2.Rule(v => v < 10).WithMessage("Nested Message S2"))
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T2"));
                
                FullModelSpecification = _ => _
                    .Member(m => m.Text1, m => m.Rule(v => v.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T1"))
                    .Member(m => m.Text2, m => m.Rule(v => v.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T2"))
                    .Member(m => m.Text3, m => m.Rule(v => v.Contains('c', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T3"))
                    .Member(m => m.Text4, m => m.Rule(v => v.Contains('d', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T4"))
                    .Member(m => m.Text5, m => m.Rule(v => v.Contains('e', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T5"))
                    .Member(m => m.Number1, m2 => m2.Rule(v => v < 10).WithMessage("Message N1"))
                    .Member(m => m.Number2, m2 => m2.Rule(v => v < 10).WithMessage("Message N2"))
                    .Member(m => m.Number3, m2 => m2.Rule(v => v < 10).WithMessage("Message N3"))
                    .Member(m => m.Number4, m2 => m2.Rule(v => v < 10).WithMessage("Message N4"))
                    .Member(m => m.Number5, m2 => m2.Rule(v => v < 10).WithMessage("Message N5"))
                    .Member(m => m.SuperNumber1, m2 => m2.Rule(v => v < 10).WithMessage("Message S1"))
                    .Member(m => m.SuperNumber2, m2 => m2.Rule(v => v < 10).WithMessage("Message S2"))
                    .Member(m => m.SuperNumber3, m2 => m2.Rule(v => v < 10).WithMessage("Message S3"))
                    .Member(m => m.NestedModel1, NestedModelSpecification)
                    .Member(m => m.NestedModel2, NestedModelSpecification)
                    .Member(m => m.ModelCollection, m => m
                        .MaxCollectionSize(10).WithMessage("No more than 10 items are allowed")
                        .AsCollection(NestedModelSpecification))
                    .Member(m => m.StructCollection, m => m
                        .MaxCollectionSize(10).WithMessage("No more than 10 items are allowed")
                        .AsCollection(m1 => m1.Rule(m2 => m2 < 10).WithMessage("Message C")));
                
            }

            void SetupFullModelManyErrorsFaker()
            {
                var nestedModelManyErrorsFaker = new Faker<NestedModel>()
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 5))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 15))
                    .RuleFor(m => m.Text1, m => m.Lorem.Word().OrNull(m, 0.1f))
                    .RuleFor(m => m.Text2, m => m.Lorem.Word().OrNull(m, 0.15f))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 20).OrNull(m, 0.20f))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 20).OrNull(m, 0.25f));

                FullModelManyErrorsFaker = new Faker<FullModel>()
                    .RuleFor(m => m.Text1, m => m.Lorem.Word().OrNull(m, 0.1f))
                    .RuleFor(m => m.Text2, m => m.Lorem.Word().OrNull(m, 0.15f))
                    .RuleFor(m => m.Text3, m => m.Lorem.Word().OrNull(m, 0.2f))
                    .RuleFor(m => m.Text4, m => m.Lorem.Word().OrNull(m, 0.25f))
                    .RuleFor(m => m.Text5, m => m.Lorem.Word().OrNull(m, 0.3f))
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 5))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 10))
                    .RuleFor(m => m.Number3, m => m.Random.Int(0, 15))
                    .RuleFor(m => m.Number4, m => m.Random.Int(0, 20))
                    .RuleFor(m => m.Number5, m => m.Random.Int(0, 25))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 20).OrNull(m, 0.20f))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 20).OrNull(m, 0.25f))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 20).OrNull(m, 0.30f))
                    .RuleFor(m => m.NestedModel1, m => nestedModelManyErrorsFaker.Generate())
                    .RuleFor(m => m.NestedModel2, m => nestedModelManyErrorsFaker.Generate())
                    .RuleFor(m => m.ModelCollection, m => nestedModelManyErrorsFaker.GenerateBetween(0, 20).ToList().OrNull(m, 0.7f))
                    .RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 20)).Select(_ => m.Random.Number(0, 20)).ToList().OrNull(m, 0.7f));
            }

            void SetupFullModelNoErrorsFaker()
            {
                var nestedModelNoErrorsFaker = new Faker<NestedModel>()
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Text1, m => m.Lorem.Word() + "a")
                    .RuleFor(m => m.Text2, m => m.Lorem.Word() + "b")
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 9))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9));

                FullModelNoErrorsFaker = new Faker<FullModel>()
                    .RuleFor(m => m.Text1, m => m.Lorem.Word() + "a")
                    .RuleFor(m => m.Text2, m => m.Lorem.Word() + "b")
                    .RuleFor(m => m.Text3, m => m.Lorem.Word() + "c")
                    .RuleFor(m => m.Text4, m => m.Lorem.Word() + "d")
                    .RuleFor(m => m.Text5, m => m.Lorem.Word() + "e")
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number3, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number4, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number5, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber3, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.NestedModel1, m => nestedModelNoErrorsFaker.Generate())
                    .RuleFor(m => m.NestedModel2, m => nestedModelNoErrorsFaker.Generate())
                    .RuleFor(m => m.ModelCollection, m => nestedModelNoErrorsFaker.GenerateBetween(0, 9).ToList())
                    .RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 9)).Select(_ => m.Random.Number(0, 9)).ToList());
            }
            
            void SetupFullModelHalfErrorsFaker()
            {
                var nestedModelsHalfErrorsFaker = new Faker<NestedModel>()
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Text1, m => string.Join(" ", m.Lorem.Words(20)))
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(15)))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Number(0, 10))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Number(0, 9).OrNull(m, 0.01f));

                FullModelHalfErrorsFaker = new Faker<FullModel>()
                    .RuleFor(m => m.Text1, m => string.Join("a", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text2, m => string.Join("b", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text3, m => string.Join("c", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text4, m => string.Join("d", m.Lorem.Words(10)))
                    .RuleFor(m => m.Text5, m => string.Join(" ", m.Lorem.Words(20)))
                    .RuleFor(m => m.Number1, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number2, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number3, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number4, m => m.Random.Int(0, 9))
                    .RuleFor(m => m.Number5, m => m.Random.Int(0, 10))
                    .RuleFor(m => m.SuperNumber1, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber2, m => m.Random.Decimal(0, 9))
                    .RuleFor(m => m.SuperNumber3, m => m.Random.Decimal(0, 10).OrNull(m, 0.01f))
                    .RuleFor(m => m.NestedModel1, m => nestedModelsHalfErrorsFaker.Generate())
                    .RuleFor(m => m.NestedModel2, m => nestedModelsHalfErrorsFaker.Generate())
                    .RuleFor(m => m.ModelCollection, m => nestedModelsHalfErrorsFaker.GenerateBetween(0, 9).ToList())
                    .RuleFor(m => m.StructCollection, m => Enumerable.Range(1, m.Random.Int(1, 11)).Select(_ => m.Random.Number(0, 9)).ToList());
            }

            SetupFullModelHalfErrorsFaker();
            SetupValidotSpecifications();
            SetupFullModelManyErrorsFaker();
            SetupFullModelNoErrorsFaker();
        }

        public class FullModel
        {
            public string Text1 { get; set; }
            public string Text2 { get; set; }
            public string Text3 { get; set; }
            public string Text4 { get; set; }
            public string Text5 { get; set; }
            
            public int Number1 { get; set; }
            
            public int Number2 { get; set; }
            
            public int Number3 { get; set; }
            
            public int Number4 { get; set; }
            
            public int Number5 { get; set; }
            
            public decimal? SuperNumber1 { get; set; }
            
            public decimal? SuperNumber2 { get; set; }
            
            public decimal? SuperNumber3 { get; set; }
            
            public NestedModel NestedModel1 { get; set; }
            
            public NestedModel NestedModel2 { get; set; }
            
            public IReadOnlyList<NestedModel> ModelCollection { get; set; }
            
            public IReadOnlyList<int> StructCollection { get; set; }
        }
        
        public class NestedModel
        {
            public string Text1 { get; set; }
            
            public string Text2 { get; set; }
            
            public int Number1 { get; set; }
            
            public int Number2 { get; set; }
            
            public decimal? SuperNumber1 { get; set; }
            
            public decimal? SuperNumber2 { get; set; }
        }
        

        public class FullModelValidator : AbstractValidator<FullModel> 
        {
            public FullModelValidator()
            {
                RuleFor(x => x.Text1).NotNull();
                RuleFor(x => x.Text1).Must(m => m.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T1").When(m => m.Text1 != null);
                
                RuleFor(x => x.Text2).NotNull();
                RuleFor(x => x.Text2).Must(m => m.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T2").When(m => m.Text2 != null);
                
                RuleFor(x => x.Text3).NotNull();
                RuleFor(x => x.Text3).Must(m => m.Contains('c', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T3").When(m => m.Text3 != null);
                
                RuleFor(x => x.Text4).NotNull();
                RuleFor(x => x.Text4).Must(m => m.Contains('d', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T4").When(m => m.Text4 != null);
                
                RuleFor(x => x.Text5).NotNull();
                RuleFor(x => x.Text5).Must(m => m.Contains('e', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Message T5").When(m => m.Text5 != null);

                RuleFor(x => x.Number1).Must(m => m < 10).WithMessage("Message N1");
                RuleFor(x => x.Number2).Must(m => m < 10).WithMessage("Message N2");
                RuleFor(x => x.Number3).Must(m => m < 10).WithMessage("Message N3");
                RuleFor(x => x.Number4).Must(m => m < 10).WithMessage("Message N4");
                RuleFor(x => x.Number5).Must(m => m < 10).WithMessage("Message N5");
                
                RuleFor(x => x.SuperNumber1).NotNull();
                RuleFor(x => x.SuperNumber1).Must(m => m < 10).WithMessage("Message S1").When(m => m.SuperNumber1 != null);

                RuleFor(x => x.SuperNumber2).NotNull();
                RuleFor(x => x.SuperNumber2).Must(m => m < 10).WithMessage("Message S2").When(m => m.SuperNumber2 != null);
                
                RuleFor(x => x.SuperNumber3).NotNull();
                RuleFor(x => x.SuperNumber3).Must(m => m < 10).WithMessage("Message S3").When(m => m.SuperNumber3 != null);

                RuleFor(x => x.NestedModel1).NotNull();
                RuleFor(x => x.NestedModel1).SetValidator(new NestedModelValidator()).When(m => m.NestedModel1 != null);

                RuleFor(x => x.NestedModel2).NotNull();
                RuleFor(x => x.NestedModel2).SetValidator(new NestedModelValidator()).When(m => m.NestedModel2 != null);

                RuleFor(x => x.ModelCollection).NotNull();
                RuleFor(x => x.ModelCollection)
                    .Must(x => x.Count <= 10).WithMessage("No more than 10 items are allowed")
                    .When(m => m.ModelCollection != null);
                
                RuleForEach(x => x.ModelCollection).SetValidator(new NestedModelValidator()).When(m => m.ModelCollection != null);

                RuleFor(x => x.StructCollection).NotNull();
                RuleFor(x => x.StructCollection)
                    .Must(x => x.Count <= 10).WithMessage("No more than 10 items are allowed")
                    .When(m => m.StructCollection != null);

                RuleForEach(x => x.StructCollection).Must(m1 => m1 <= 10).WithMessage("Message C").When(m => m.StructCollection != null);

            }
        }
        
        public class NestedModelValidator : AbstractValidator<NestedModel> 
        {
            public NestedModelValidator()
            { 
                RuleFor(x => x.Text1).NotNull();
                RuleFor(x => x.Text1).Must(m => m.Contains('a', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T1").When(m => m.Text1 != null);
                
                RuleFor(x => x.Text2).NotNull();
                RuleFor(x => x.Text2).Must(m => m.Contains('b', StringComparison.InvariantCultureIgnoreCase)).WithMessage("Nested Message T2").When(m => m.Text2 != null);
                
                RuleFor(x => x.Number1).Must(m => m < 10).WithMessage("Nested Message N1");
                RuleFor(x => x.Number2).Must(m => m < 10).WithMessage("Nested Message N2");
                
                RuleFor(x => x.SuperNumber1).NotNull();
                RuleFor(x => x.SuperNumber1).Must(m => m < 10).WithMessage("Nested Message S1").When(m => m.SuperNumber1 != null);

                RuleFor(x => x.SuperNumber2).NotNull();
                RuleFor(x => x.SuperNumber2).Must(m => m < 10).WithMessage("Nested Message S2").When(m => m.SuperNumber2 != null);
            }
        }
        
        public static Specification<FullModel> FullModelSpecification { get; private set; }
        public static Specification<NestedModel> NestedModelSpecification { get; private set;}
        
        public static Faker<FullModel> FullModelManyErrorsFaker { get; private set;} 
        public static Faker<FullModel> FullModelNoErrorsFaker { get; private set;} 
            
        public static Faker<FullModel> FullModelHalfErrorsFaker { get; private set;} 
        
    }
}
