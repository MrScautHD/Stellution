
			float3 interpolation_c2( float3 x ) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }

			float3 mod(float3 x, float3 y)
			{
				return x - y * floor(x / y);
			}

			float4 mod(float4 x, float4 y)
			{
				return x - y * floor(x / y);
			}

			float3 mod289(float3 x)
			{
				return x - floor(x / 289.0) * 289.0;
			}

			float4 mod289(float4 x)
			{
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			float4 permute(float4 x)
			{
				return mod289(((x*34.0) + 1.0)*x);
			}

			float3 fade(float3 t) {
				return t*t*t*(t*(t*6.0 - 15.0) + 10.0);
			}
			
			float4 taylorInvSqrt(float4 r)
			{
				return 1.79284291400159 - 0.85373472095314 * r;
			}

			float2 fade(float2 t) {
				return t*t*t*(t*(t*6.0 - 15.0) + 10.0);
			}

			////

			float Falloff_Xsq_C2(float xsq) { xsq = 1.0 - xsq; return xsq*xsq*xsq; }	// ( 1.0 - x*x )^3.   NOTE: 2nd derivative is 0.0 at x=1.0, but non-zero at x=0.0
			float4 Falloff_Xsq_C2(float4 xsq) { xsq = 1.0 - xsq; return xsq*xsq*xsq; }
			float2 Interpolation_C2(float2 x) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }


			void FAST32_hash_2D(float2 gridcell, out float4 hash_0, out float4 hash_1)	//	generates 2 random numbers for each of the 4 cell corners
			{
				//    gridcell is assumed to be an integer coordinate
				const float2 OFFSET = float2(26.0, 161.0);
				const float DOMAIN = 71.0;
				const float2 SOMELARGEFLOATS = float2(951.135664, 642.949883);
				float4 P = float4(gridcell.xy, gridcell.xy + 1.0);
				P = P - floor(P * (1.0 / DOMAIN)) * DOMAIN;
				P += OFFSET.xyxy;
				P *= P;
				P = P.xzxz * P.yyww;
				hash_0 = frac(P * (1.0 / SOMELARGEFLOATS.x));
				hash_1 = frac(P * (1.0 / SOMELARGEFLOATS.y));
			}

			float4 FAST32_hash_2D(float2 gridcell)	//	generates a random number for each of the 4 cell corners
			{
				//	gridcell is assumed to be an integer coordinate
				const float2 OFFSET = float2(26.0, 161.0);
				const float DOMAIN = 71.0;
				const float SOMELARGEFLOAT = 951.135664;
				float4 P = float4(gridcell.xy, gridcell.xy + 1.0);
				P = P - floor(P * (1.0 / DOMAIN)) * DOMAIN;	//	truncate the domain
				P += OFFSET.xyxy;								//	offset to interesting part of the noise
				P *= P;											//	calculate and return the hash
				return frac(P.xzxz * P.yyww * (1.0 / SOMELARGEFLOAT));
			}


			//
			//	Perlin Noise 2D  ( gradient noise )
			//	Return value range of -1.0->1.0
			//	http://briansharpe.files.wordpress.com/2011/11/perlinsample.jpg
			//
			float Perlin2D(float2 P)
			{
				//	establish our grid cell and unit position
				float2 Pi = floor(P);
				float4 Pf_Pfmin1 = P.xyxy - float4(Pi, Pi + 1.0);

#if CLASSICPERLIN
				//
				//	classic noise looks much better than improved noise in 2D, and with an efficent hash function runs at about the same speed.
				//	requires 2 random numbers per point.
				//

				//	calculate the hash.
				//	( various hashing methods listed in order of speed )
				float4 hash_x, hash_y;
				FAST32_hash_2D(Pi, hash_x, hash_y);
				//SGPP_hash_2D( Pi, hash_x, hash_y );

				//	calculate the gradient results
				float4 grad_x = hash_x - 0.49999;
				float4 grad_y = hash_y - 0.49999;
				float4 grad_results = rsqrt(grad_x * grad_x + grad_y * grad_y) * (grad_x * Pf_Pfmin1.xzxz + grad_y * Pf_Pfmin1.yyww);

#if CLASSICPERLIN
				//	Classic Perlin Interpolation
				grad_results *= 1.4142135623730950488016887242097;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/sqrt(0.5)
				float2 blend = Interpolation_C2(Pf_Pfmin1.xy);
				float4 blend2 = float4(blend, float2(1.0 - blend));
				return dot(grad_results, blend2.zxzx * blend2.wwyy);
#else
				//	Classic Perlin Surflet
				//	http://briansharpe.wordpress.com/2012/03/09/modifications-to-classic-perlin-noise/
				grad_results *= 2.3703703703703703703703703703704;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/cube(0.75)
				float4 vecs_len_sq = Pf_Pfmin1 * Pf_Pfmin1;
				vecs_len_sq = vecs_len_sq.xzxz + vecs_len_sq.yyww;
				return dot(Falloff_Xsq_C2(min(float4(1.0), vecs_len_sq)), grad_results);
#endif

#else
				//
				//	2D improved perlin noise.
				//	requires 1 random value per point.
				//	does not look as good as classic in 2D due to only a small number of possible cell types.  But can run a lot faster than classic perlin noise if the hash function is slow
				//

				//	calculate the hash.
				//	( various hashing methods listed in order of speed )
				float4 hash = FAST32_hash_2D(Pi);
				//vec4 hash = BBS_hash_2D( Pi );
				//vec4 hash = SGPP_hash_2D( Pi );
				//vec4 hash = BBS_hash_hq_2D( Pi );

				//
				//	evaulate the gradients
				//	choose between the 4 diagonal gradients.  ( slightly slower than choosing the axis gradients, but shows less grid artifacts )
				//	NOTE:  diagonals give us a nice strict -1.0->1.0 range without additional scaling
				//	[1.0,1.0] [-1.0,1.0] [1.0,-1.0] [-1.0,-1.0]
				//
				hash -= 0.5;
				float4 grad_results = Pf_Pfmin1.xzxz * sign(hash) + Pf_Pfmin1.yyww * sign(abs(hash) - 0.25);

				//	blend the results and return
				float2 blend = Interpolation_C2(Pf_Pfmin1.xy);
				float4 blend2 = float4(blend, float2(1.0 - blend));
				return dot(grad_results, blend2.zxzx * blend2.wwyy);

#endif
			}

			//	convert a 0.0->1.0 sample to a -1.0->1.0 sample weighted towards the extremes
			float4 Cellular_weight_samples(float4 samples)
			{
				samples = samples * 2.0 - 1.0;
				//return (1.0 - samples * samples) * sign(samples);	// square
				return (samples * samples * samples) - sign(samples);	// cubic (even more variance)
			}

			float Cellular2D(float2 P)
			{
				//	establish our grid cell and unit position
				float2 Pi = floor(P);
				float2 Pf = P - Pi;

				//	calculate the hash.
				//	( various hashing methods listed in order of speed )
				float4 hash_x, hash_y;
				FAST32_hash_2D(Pi, hash_x, hash_y);
				//SGPP_hash_2D( Pi, hash_x, hash_y );

				//	generate the 4 random points
#if WORLEY_1
				//	restrict the random point offset to eliminate artifacts
				//	we'll improve the variance of the noise by pushing the points to the extremes of the jitter window
				const float JITTER_WINDOW = 0.25;	// 0.25 will guarentee no artifacts.  0.25 is the intersection on x of graphs f(x)=( (0.5+(0.5-x))^2 + (0.5-x)^2 ) and f(x)=( (0.5+x)^2 + x^2 )
				hash_x = Cellular_weight_samples(hash_x) * JITTER_WINDOW + float4(0.0, 1.0, 0.0, 1.0);
				hash_y = Cellular_weight_samples(hash_y) * JITTER_WINDOW + float4(0.0, 0.0, 1.0, 1.0);
#else
				//	non-weighted jitter window.  jitter window of 0.4 will give results similar to Stefans original implementation
				//	nicer looking, faster, but has minor artifacts.  ( discontinuities in signal )
				const float JITTER_WINDOW = 0.4;
				hash_x = hash_x * JITTER_WINDOW * 2.0 + float4(-JITTER_WINDOW, 1.0 - JITTER_WINDOW, -JITTER_WINDOW, 1.0 - JITTER_WINDOW);
				hash_y = hash_y * JITTER_WINDOW * 2.0 + float4(-JITTER_WINDOW, -JITTER_WINDOW, 1.0 - JITTER_WINDOW, 1.0 - JITTER_WINDOW);
#endif

				//	return the closest squared distance
				float4 dx = Pf.xxxx - hash_x;
				float4 dy = Pf.yyyy - hash_y;
				float4 d = dx * dx + dy * dy;
				d.xy = min(d.xy, d.zw);
				return min(d.x, d.y) * (1.0 / 1.125);	//	scale return value from 0.0->1.125 to 0.0->1.0  ( 0.75^2 * 2.0  == 1.125 )
			} 

			 

			float CalculateWorley3oct(float2 p, float p1, float p2, float p3) {
				float2 xy = p * p1;
				float2 xy2 = p * p2;
				float2 xy3 = p * p3;
				   
				float worley_value1 = Cellular2D(xy).r;
				float worley_value2 = Cellular2D(xy2).r;
				float worley_value3 = Cellular2D(xy3).r;

				worley_value1 = worley_value1;
				worley_value2 = worley_value2;
				worley_value3 = worley_value3; 
				 
				float worley_value = worley_value1 * 3;  
				worley_value = worley_value + worley_value2 * 1.5;
				worley_value = worley_value + worley_value3 * 1.5;
				    
				return saturate(1 - worley_value);
			}

			float CalculateWorley1(float2 p, float p1) {
				float2 xy = p * p1;
				float worley_value1 = Cellular2D(xy).r;
				worley_value1 = worley_value1;
				return saturate(1 - worley_value1);
			}


			float CalculatePerlin5(float2 p) 
			{

				float2 xy = p;
				float amplitude_factor = 0.5;
				float frequency_factor = 2.0;

				float a = 1.0;
				float perlin_value = 0.0;
				perlin_value += a * Perlin2D(xy).r; a *= amplitude_factor; xy *= (frequency_factor + 0.12);
				perlin_value -= a * Perlin2D(xy).r; a *= amplitude_factor; xy *= (frequency_factor + 0.03);
				perlin_value -= a * Perlin2D(xy).r; a *= amplitude_factor; xy *= (frequency_factor + 0.01);
				perlin_value -= a * Perlin2D(xy).r; a *= amplitude_factor; xy *= (frequency_factor + 0.01);
				perlin_value += a * Perlin2D(xy).r;

				return perlin_value;
			}



			// Classic Perlin noise, periodic variant
			float penoise(float2 P, float2 rep)
			{
				float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
				float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);
				Pi = mod(Pi, rep.xyxy); // To create noise with explicit period
				Pi = mod289(Pi);        // To avoid truncation effects in permutation
				float4 ix = Pi.xzxz;
				float4 iy = Pi.yyww;
				float4 fx = Pf.xzxz;
				float4 fy = Pf.yyww;

				float4 i = permute(permute(ix) + iy);

				float4 gx = frac(i * (1.0 / 41.0)) * 2.0 - 1.0;
				float4 gy = abs(gx) - 0.5;
				float4 tx = floor(gx + 0.5);
				gx = gx - tx;

				float2 g00 = float2(gx.x, gy.x);
				float2 g10 = float2(gx.y, gy.y);
				float2 g01 = float2(gx.z, gy.z);
				float2 g11 = float2(gx.w, gy.w);

				float4 norm = taylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
				g00 *= norm.x;
				g01 *= norm.y;
				g10 *= norm.z;
				g11 *= norm.w;

				float n00 = dot(g00, float2(fx.x, fy.x));
				float n10 = dot(g10, float2(fx.y, fy.y));
				float n01 = dot(g01, float2(fx.z, fy.z));
				float n11 = dot(g11, float2(fx.w, fy.w));

				float2 fade_xy = fade(Pf.xy);
				float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
				float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
				return 2.3 * n_xy;
			}

			float CalculatePerlinTileing5(float2 p, float2 rep)
			{

				float2 xy = p;
				float amplitude_factor = 0.5;
				float frequency_factor = 1.0;

				float a = 1.0;
				float perlin_value = 0.0;
				perlin_value += a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value += a * penoise(xy, rep).r;

				return perlin_value;
			}

			float CalculatePerlinTileing5OLD(float2 p, float2 rep)
			{

				float2 xy = p;
				float amplitude_factor = 0.5;
				float frequency_factor = 2.0;

				float a = 1.0;
				float perlin_value = 0.0;
				perlin_value += a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 0.12);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 0.03);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 0.01);
				perlin_value -= a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 0.01);
				perlin_value += a * penoise(xy, rep).r;

				return perlin_value;
			}


		/*	float CalculatePerlinTileing(float2 p, float2 rep)
			{
				float perlin_value = 0.0;
				//float2 period = rep;
				float2 xy = p;
				float w = 1.0;
				float s = 1.0;

				for (int i = 0; i < 6; i++)
				{
					float2 coord = p * s;
					float2 period = s * 2.0;

					perlin_value += penoise(coord, period) * w;
				
					w *= 0.5;
					s *= 0.25;
					period *= s;
				}

				return perlin_value;
			}
			*/

			float CalculatePerlinTileing(float2 p, float2 rep)
			{

				float2 xy = p;
				float amplitude_factor = 0.5;
				float frequency_factor = 1.0;

				float a = 1.0;
				float perlin_value = 0.0;
				perlin_value += a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value += a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 1);
				perlin_value += a * penoise(xy, rep).r; a *= amplitude_factor; xy *= (frequency_factor + 2);
				perlin_value -= a * penoise(xy, rep).r;

				return perlin_value;
			}