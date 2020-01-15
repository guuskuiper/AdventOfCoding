import re

with open('input.txt') as file:
    input = file.read()

rules = input.split('\n')

print(rules)

def shuffle(deck, rules):
    for s in rules:
        if s == 'deal into new stack':
            deck.reverse()
            continue
        match = re.match(r'cut (-?\d+)', s)
        if match != None:
            n = int(match.group(1))
            deck = deck[n:]+deck[:n]
            continue
        match = re.match(r'deal with increment (\d+)', s)
        if match != None:
            n = int(match.group(1))
            ndeck = [0]*len(deck)
            for i in range(len(deck)):
                ndeck[(i*n)%len(deck)] = deck[i]
            deck = ndeck
            continue
        raise Exception('unknown rule', s)
    return deck

deck = shuffle(list(range(10007)), rules)
deck.index(2019)

# convert rules to linear polynomial.
# (g∘f)(x) = g(f(x))
def parse(L, rules):
    a,b = 1,0
    for s in rules[::-1]:
        if s == 'deal into new stack':
            a = -a
            b = L-b-1
            print('y = {}*x + {} DEALINTO'.format(a, b))
            continue
        if s.startswith('cut'):
            n = int(s.split(' ')[1])
            b = (b+n)%L
            print('y = {}*x + {} CUT {}'.format(a, b, n))
            continue
        if s.startswith('deal with increment'):
            n = int(s.split(' ')[3])
            z = pow(n,L-2,L) # == modinv(n,L)
            a = a*z % L
            b = b*z % L
            print('y = {}*x + {} DEALINCREMENT {} INVMOD {}'.format(a, b, n, z))
            continue
        raise Exception('unknown rule', s)
    return a,b

# modpow the polynomial: (ax+b)^m % n
# f(x) = ax+b
# g(x) = cx+d
# f^2(x) = a(ax+b)+b = aax + ab+b
# f(g(x)) = a(cx+d)+b = acx + ad+b
def polypow(a,b,m,n):
    if m==0:
        return 1,0
    if m%2==0:
        return polypow(a*a%n, (a*b+b)%n, m//2, n)
    else:
        c,d = polypow(a,b,m-1,n)
        return a*c%n, (a*d+b)%n

def shuffle2(L, N, pos, rules):
    a,b = parse(L,rules)
    print(a, b)
    a,b = polypow(a,b,N,L)
    print(a, b)
    return (pos*a+b)%L
	
#%%time

# test it out
pos = 8502
L = 10007
N = 1
deck = list(range(L))
for i in range(N):
    deck = shuffle(deck,rules)
print(deck[pos])
#print(shuffle2(L,N,pos,rules))

L = 119315717514047
N = 101741582076661
print(shuffle2(L,N,2020,rules))

print(polypow(69146477285364, 50766547354351, N, L))